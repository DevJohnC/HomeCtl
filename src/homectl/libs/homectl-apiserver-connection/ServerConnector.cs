using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Manages a configured connection to the API server.
	/// </summary>
	public class ServerConnector
	{
		private readonly IApiClientProvider[] _clientProviders;
		private readonly IServerLivelinessMonitor[] _monitors;
		private readonly TimeSpan _connectAttemptCooldown = TimeSpan.FromSeconds(5);

		public ConnectionStates ConnectionState { get; private set; }

		public ServerConnector(
			IEnumerable<IApiClientProvider> clientProviders,
			IEnumerable<IServerLivelinessMonitor> monitors
			)
		{
			_clientProviders = clientProviders.ToArray();
			_monitors = monitors.ToArray();
		}

		public async Task Run(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				switch (ConnectionState)
				{
					case ConnectionStates.NotConnected:
						try
						{
							await CreateClient(stoppingToken);
						}
						catch
						{
							//  failed to connect, wait a while and try again
							await Task.Delay(_connectAttemptCooldown);
						}
						break;
					case ConnectionStates.Connected:
						try
						{
							await MonitorForDisconnect(stoppingToken);
						}
						catch
						{
							//  monitoring failure, assume disconnected
							ConnectionState = ConnectionStates.NotConnected;
						}
						break;
				}
			}
		}

		private async Task CreateClient(CancellationToken stoppingToken)
		{
			var cancellationSource = new CancellationTokenSource();
			var combinedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
				stoppingToken, cancellationSource.Token
				);
			var connectingTasks = _clientProviders
				.Select(q => CreateApiClient(q, combinedCancellationSource.Token))
				.ToList();
			var exceptions = new List<Exception>();

			while (!stoppingToken.IsCancellationRequested &&
				ConnectionState != ConnectionStates.Connected)
			{
				try
				{
					var finishedTask = await Task.WhenAny(connectingTasks);
					connectingTasks.Remove(finishedTask);

					var (provider, httpClient) = await finishedTask;
					cancellationSource.Cancel();
					ConnectionState = ConnectionStates.Connected;
				}
				catch (Exception ex)
				{
					//  todo: log
					exceptions.Add(ex);
					if (connectingTasks.Count == 0)
						throw new AggregateException("All connection methods failed.", exceptions);
				}
			}
		}

		private async Task<(IApiClientProvider Provider, HttpClient Client)> CreateApiClient(
			IApiClientProvider provider, CancellationToken stoppingToken)
		{
			var client = await provider.CreateApiClient(stoppingToken);
			if (stoppingToken.IsCancellationRequested)
				return default;
			return (provider, client);
		}

		private async Task MonitorForDisconnect(CancellationToken stoppingToken)
		{
			var cancellationSource = new CancellationTokenSource();
			var combinedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
				stoppingToken, cancellationSource.Token
				);

			try
			{
				var monitoringTasks = _monitors
				.Select(q => q.MonitorForDisconnect(combinedCancellationSource.Token))
				.ToArray();

				var finishedTask = await Task.WhenAny(monitoringTasks);
				//  await the finished task to observe any exceptions
				await finishedTask;
			}
			catch (Exception ex)
			{
				//  todo: log exception
				throw;
			}
			finally
			{
				cancellationSource.Cancel();
				ConnectionState = ConnectionStates.NotConnected;
			}
		}

		public enum ConnectionStates
		{
			/// <summary>
			/// No connection to the API server is available.
			/// </summary>
			NotConnected,
			/// <summary>
			/// A connection to the API server is configured and available.
			/// </summary>
			/// <remarks>
			/// A connected state doesn't mean there's an active network connection to the API server;
			/// rather it means that a grpc channel/http client configured to make requests to the API
			/// server's endpoint is setup for use.
			/// </remarks>
			Connected
		}
	}
}
