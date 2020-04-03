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

		private CancellationTokenSource _connectCycleCooldownCancellation = new CancellationTokenSource();
		private MonitoredGrpcChannel? _grpcChannel;

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
		public ChannelBase? GrpcChannel => _grpcChannel;

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
						_grpcChannel = CreateGrpcChannel(result.Client);
						Connected?.Invoke(this, new ConnectionEventArgs(GrpcChannel));
					}
				}

				await ConnectionAttemptCooldown(_connectCycleTimeout, cancellationToken);
			}

			await Shutdown();
		}

		/// <summary>
		/// Back off on connection attempts for a period of time.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task ConnectionAttemptCooldown(TimeSpan timeSpan, CancellationToken cancellationToken)
		{
			try
			{
				using (var combinedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
						_connectCycleCooldownCancellation.Token,
						cancellationToken))
				{
					await Task.Delay(timeSpan, cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				CheckCooldownCancellationSource();
			}
		}

		/// <summary>
		/// Replaces the cooldown cancellation token if needed.
		/// </summary>
		/// <remarks>
		/// Only call from within the Run method to avoid race conditions.
		/// </remarks>
		private void CheckCooldownCancellationSource()
		{
			if (_connectCycleCooldownCancellation.IsCancellationRequested)
			{
				//  copy the reference to _connectCycleCooldownCancellation so that we can replace it
				//  in place (so that __connectCycleCooldownCancellation.Cancel() can't throw NRE)
				//  and still be capable of disposal
				var oldTokenSource = _connectCycleCooldownCancellation;
				_connectCycleCooldownCancellation = new CancellationTokenSource();
				oldTokenSource.Dispose();
			}
		}

		/// <summary>
		/// Shutdown the connection.
		/// </summary>
		/// <returns></returns>
		private async Task Shutdown()
		{
			await (_grpcChannel?.ShutdownAsync() ?? Task.CompletedTask);
			Disconnect(_grpcChannel, null);
		}

		/// <summary>
		/// Handle disconnect for a grpc channel.
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="exception"></param>
		private void Disconnect(MonitoredGrpcChannel? channel, Exception? exception)
		{
			if (channel != null)
			{
				//  remove the InvokeError event handler to prevent Disconnect bouncing
				//  (ie, if multiple gRPC invocations fail at once)
				channel.InvokeError -= ClientInvokeError;
				ConnectionState = ConnectionStates.Disconnected;
				_grpcChannel = null;
				Disconnected?.Invoke(this, new ConnectionEventArgs(channel, exception));
				_connectCycleCooldownCancellation.Cancel();
			}
		}

		/// <summary>
		/// Creates a grpc channel ready to invoke grpc methods on the remote host.
		/// </summary>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		private MonitoredGrpcChannel CreateGrpcChannel(HttpClient httpClient)
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
			if (eventArgs.Exception is HttpRequestException ||
				(eventArgs.Exception is RpcException rpcEx && rpcEx.Status.StatusCode == StatusCode.Internal))
			{
				var monitoredChannel = (MonitoredGrpcChannel)sender;
				Disconnect(monitoredChannel, eventArgs.Exception);
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
