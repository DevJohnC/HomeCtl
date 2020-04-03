using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Manages connections to an API server.
	/// </summary>
	public sealed class ApiServerConnectionManager
	{
		private readonly IConnectionProvider[] _connectionProviders;
		private readonly TimeSpan _connectCycleTimeout = TimeSpan.FromSeconds(1);
		private readonly TimeSpan _connectAttemptTimeout = TimeSpan.FromSeconds(5);

		public ApiServerConnectionManager(IEnumerable<IConnectionProvider> connectionProviders)
		{
			_connectionProviders = connectionProviders.ToArray();
		}

		/// <summary>
		/// Gets the current connection state.
		/// </summary>
		public ConnectionStates ConnectionState { get; private set; }

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
						var grpcChannel = GrpcChannel.ForAddress(result.Client.BaseAddress, new GrpcChannelOptions
						{
							DisposeHttpClient = false,
							HttpClient = result.Client
						});
					}
				}

				await Task.Delay(_connectCycleTimeout, cancellationToken);
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
