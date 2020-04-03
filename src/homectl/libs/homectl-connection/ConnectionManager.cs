using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Contains event data for a connection event.
	/// </summary>
	public class ConnectionEventArgs : EventArgs
	{
		public ConnectionEventArgs(ChannelBase grpcChannel)
		{
			GrpcChannel = grpcChannel;
		}

		public ConnectionEventArgs(ChannelBase grpcChannel, Exception? exception) :
			this(grpcChannel)
		{
			Exception = exception;
		}

		/// <summary>
		/// Gets the grpc channel connected/disconnected to/from the remote endpoint.
		/// </summary>
		public ChannelBase GrpcChannel { get; }

		/// <summary>
		/// Gets the exception that caused a disconnect.
		/// </summary>
		public Exception? Exception { get; }
	}

	/// <summary>
	/// Manages connections to an API server.
	/// </summary>
	public sealed class ConnectionManager
	{
		private readonly IConnectionProvider[] _connectionProviders;
		private readonly TimeSpan _connectCycleTimeout = TimeSpan.FromSeconds(1);
		private readonly TimeSpan _connectAttemptTimeout = TimeSpan.FromSeconds(5);

		public ConnectionManager(IEnumerable<IConnectionProvider> connectionProviders)
		{
			_connectionProviders = connectionProviders.ToArray();
		}

		/// <summary>
		/// Event raised when a new connection is established.
		/// </summary>
		public event EventHandler<ConnectionEventArgs>? Connected;

		/// <summary>
		/// Event raised when a connection fails.
		/// </summary>
		public event EventHandler<ConnectionEventArgs>? Disconnected;

		/// <summary>
		/// Gets the current connection state.
		/// </summary>
		public ConnectionStates ConnectionState { get; private set; }

		/// <summary>
		/// Gets the current grpc channel if connected.
		/// </summary>
		public ChannelBase? GrpcChannel { get; private set; }

		/// <summary>
		/// Runs the connection manager.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task Run(CancellationToken cancellationToken = default)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (ConnectionState != ConnectionStates.Connected)
				{
					var result = await AttemptToConnect(cancellationToken);
					if (result.WasConnectionEstablished &&
						result.Client != null)
					{
						ConnectionState = ConnectionStates.Connected;
						GrpcChannel = CreateGrpcChannel(result.Client);
						Connected?.Invoke(this, new ConnectionEventArgs(GrpcChannel));
					}
				}

				await Task.Delay(_connectCycleTimeout, cancellationToken);
			}
		}

		/// <summary>
		/// Creates a grpc channel ready to invoke grpc methods on the remote host.
		/// </summary>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		private ChannelBase CreateGrpcChannel(HttpClient httpClient)
		{
			var implClient = Grpc.Net.Client.GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions
			{
				DisposeHttpClient = false,
				HttpClient = httpClient
			});
			var monitoredClient = new MonitoredGrpcChannel(implClient);
			monitoredClient.InvokeError += ClientInvokeError;
			return monitoredClient;
		}

		/// <summary>
		/// Client invoke error handler.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ClientInvokeError(object sender, CallInvokerErrorEventArgs eventArgs)
		{
			if (eventArgs.Exception is HttpRequestException)
			{
				ConnectionState = ConnectionStates.Disconnected;
				GrpcChannel = null;
				Disconnected?.Invoke(this, new ConnectionEventArgs((ChannelBase)sender, eventArgs.Exception));
			}
		}

		/// <summary>
		/// Attempt to make a connection using the connection providers available.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task<ConnectionResult> AttemptToConnect(CancellationToken cancellationToken)
		{
			foreach (var connectionProvider in _connectionProviders)
			{
				using (var attemptTimeoutSource = new CancellationTokenSource(_connectAttemptTimeout))
				using (var combinedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
						attemptTimeoutSource.Token,
						cancellationToken))
				{
					var result = await connectionProvider.AttemptConnection(combinedCancellationSource.Token);
					if (result.WasConnectionEstablished)
						return result;
				}
			}
			return ConnectionResult.Failed;
		}

		/// <summary>
		/// Connection status.
		/// </summary>
		public enum ConnectionStates
		{
			Disconnected,
			Connected
		}
	}
}
