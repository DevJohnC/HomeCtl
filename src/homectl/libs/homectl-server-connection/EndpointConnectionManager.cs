using HomeCtl.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public class EndpointConnectionManager
	{
		protected readonly EventBus EventBus;
		private readonly IEndpointClientFactory _endpointClientFactory;
		private readonly IServerIdentityVerifier _serverIdentifyVerifier;
		private readonly ILogger<EndpointConnectionManager> _logger;

		public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.NotConnected;
		public ServerEndpoint Endpoint { get; private set; }
		public ServicesChannel? ServicesChannel { get; private set; }

		public EndpointConnectionManager(EventBus eventBus,
			IEndpointClientFactory endpointClientFactory,
			IServerIdentityVerifier serverIdentifyVerifier,
			ILogger<EndpointConnectionManager> logger)
		{
			EventBus = eventBus;
			_endpointClientFactory = endpointClientFactory;
			_serverIdentifyVerifier = serverIdentifyVerifier;
			_logger = logger;
		}

		private void SetDisconnectedState()
		{
			var previousStatus = ConnectionStatus;
			ConnectionStatus = ConnectionStatus.NotConnected;

			if (previousStatus == ConnectionStatus.Connected)
			{
				_logger.LogDebug($"Disconnected from {Endpoint.Uri}");
				EventBus.Publish(
					new EndpointConnectionEvents.Disconnected(this, Endpoint)
					);
			}
		}

		private void SetConnectedState()
		{
			var previousStatus = ConnectionStatus;
			ConnectionStatus = ConnectionStatus.Connected;

			if (previousStatus == ConnectionStatus.NotConnected)
			{
				_logger.LogDebug($"Connected established with {Endpoint.Uri}");
				EventBus.Publish(
					new EndpointConnectionEvents.Connected(this, Endpoint)
					);
			}
		}

		public async Task Run(IEnumerable<IServerEndpointProvider> endpointProviders,
			IEnumerable<IServerLivelinessMonitor> livelinessMonitors,
			CancellationToken stoppingToken)
		{
			var endpointRunners = endpointProviders?.Select(
				q => new EndpointProviderRunner(q, _logger))?.ToArray();
			var livelinessMonitorsArray = livelinessMonitors?.ToArray();
			if (endpointRunners == null || endpointRunners.Length == 0)
				throw new Exception("Impossible to locate server, provide 1 or more endpoint providers.");
			if (livelinessMonitorsArray == null || livelinessMonitorsArray.Length == 0)
				throw new Exception("Impossible to monitor server connection, provide 1 or more liveliness monitors.");

			while (!stoppingToken.IsCancellationRequested)
			{
				switch (ConnectionStatus)
				{
					case ConnectionStatus.NotConnected:
						await ConnectToServer(endpointRunners, stoppingToken);
						break;
					case ConnectionStatus.Connected:
						await WaitForDisconnect(livelinessMonitorsArray, stoppingToken);
						break;
				}
			}
		}

		private async Task ConnectToServer(EndpointProviderRunner[] endpointRunners, CancellationToken stoppingToken)
		{
			using (var connectedCancelTokenSource = new CancellationTokenSource())
			using (var sessionTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
				stoppingToken, connectedCancelTokenSource.Token
				))
			{
				while (!stoppingToken.IsCancellationRequested &&
					ConnectionStatus != ConnectionStatus.Connected)
				{
					await AttemptNextConnection(endpointRunners, sessionTokenSource.Token);
				}

				if (!stoppingToken.IsCancellationRequested)
					connectedCancelTokenSource.Cancel();
			}
		}

		private async Task AttemptNextConnection(EndpointProviderRunner[] endpointRunners, CancellationToken stoppingToken)
		{
			var (serverEndpoint, endpointRunner) = await GetNextEndpoint(endpointRunners, stoppingToken);
			try
			{
				await AttemptEndpointConnection(endpointRunner, serverEndpoint, stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Connection attempt to {serverEndpoint} encountered an exception.");
			}

			if (ConnectionStatus != ConnectionStatus.Connected)
			{
				_logger.LogDebug($"Connection to {serverEndpoint.Uri} failed, waiting 5 seconds");
				endpointRunner.CooldownFor(TimeSpan.FromSeconds(5));
			}
		}

		private async Task AttemptEndpointConnection(EndpointProviderRunner endpointRunner, ServerEndpoint serverEndpoint,
			CancellationToken stoppingToken)
		{
			_logger.LogDebug($"Attempting to connect with {serverEndpoint.Uri}");

			var httpClient = _endpointClientFactory.CreateHttpClient(serverEndpoint);
			if (!(await _serverIdentifyVerifier.VerifyServer(serverEndpoint, httpClient, stoppingToken)))
				return;

			if (stoppingToken.IsCancellationRequested)
				return;

			Endpoint = serverEndpoint;
			ServicesChannel = new ServicesChannel(EventBus, serverEndpoint, Grpc.Net.Client.GrpcChannel.ForAddress(
				serverEndpoint.Uri, new Grpc.Net.Client.GrpcChannelOptions
				{
					DisposeHttpClient = false,
					HttpClient = httpClient
				}));

			SetConnectedState();
		}

		private async Task<(ServerEndpoint ServerEndpoint, EndpointProviderRunner EndpointProvider)> GetNextEndpoint(
			EndpointProviderRunner[] endpointRunners, CancellationToken stoppingToken
			)
		{
			var endpointResolveTasks = endpointRunners
				.Select(q => q.GetEndpoint(stoppingToken))
				.ToList();

			var finishedTask = await Task.WhenAny(endpointResolveTasks);

			var result = await finishedTask;
			result.ProviderRunner.Reset();
			return result;
		}

		private async Task WaitForDisconnect(IServerLivelinessMonitor[] livelinessMonitors, CancellationToken stoppingToken)
		{
			_logger.LogDebug($"Monitoring connection to {Endpoint.Uri} for disconnects");

			using (var stopMonitoringSource = new CancellationTokenSource())
			using (var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
					stoppingToken, stopMonitoringSource.Token
					))
			{
				var monitorTasks = livelinessMonitors
					.Select(q => RunLivelinessMonitor(q, cancellationSource.Token))
					.ToList();

				await Task.WhenAny(monitorTasks);

				SetDisconnectedState();

				Endpoint = default;
				await (ServicesChannel?.ShutdownAsync() ?? Task.CompletedTask);
				ServicesChannel = null;

				stopMonitoringSource.Cancel();
			}
		}

		private async Task RunLivelinessMonitor(IServerLivelinessMonitor livelinessMonitor, CancellationToken stoppingToken)
		{
			try
			{
				await livelinessMonitor.MonitorForDisconnect(Endpoint, stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Livelieness monitor `{livelinessMonitor.GetType().FullName}` encountered an exception.");
			}
		}

		private class EndpointProviderRunner
		{
			public EndpointProviderRunner(IServerEndpointProvider serverEndpointProvider, ILogger<EndpointConnectionManager> logger)
			{
				ServerEndpointProvider = serverEndpointProvider;
				_logger = logger;
			}

			public IServerEndpointProvider ServerEndpointProvider { get; }

			private readonly ILogger<EndpointConnectionManager> _logger;
			private Task<(ServerEndpoint ServerEndpoint, EndpointProviderRunner ProviderRunner)>? _runningTask;
			private DateTime? _cooldownUntil;
			private readonly object _lock = new object();

			private async Task WaitForCooldown(CancellationToken stoppingToken)
			{
				if (_cooldownUntil == null || _cooldownUntil.Value <= DateTime.Now)
					return;

				var waitTime = _cooldownUntil.Value - DateTime.Now;

				try
				{
					await Task.Delay(waitTime);
				}
				//  Task.Delay throws exceptions when cancelled via stoppingToken :/
				catch { }
			}

			private async Task<(ServerEndpoint ServerEndpoint, EndpointProviderRunner ProviderRunner)> Run(CancellationToken stoppingToken)
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					await WaitForCooldown(stoppingToken);

					try
					{
						var endpoint = await ServerEndpointProvider.GetServerEndpoint(stoppingToken);
						return (endpoint, this);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, $"Server endpoint provider `{ServerEndpointProvider.GetType().FullName}` encountered an exception.");
					}
				}

				return (default, this);
			}

			public Task<(ServerEndpoint ServerEndpoint, EndpointProviderRunner ProviderRunner)> GetEndpoint(CancellationToken stoppingToken)
			{
				lock (_lock)
				{
					if (_runningTask == null)
					{
						_runningTask = Run(stoppingToken);
					}

					return _runningTask;
				}
			}

			public void Reset()
			{
				lock (_lock)
				{
					_runningTask = null;
				}
			}

			public void CooldownFor(TimeSpan time)
			{
				_cooldownUntil = DateTime.Now + time;
			}
		}
	}

	public static class EndpointConnectionEvents
	{
		public class Connected
		{
			public Connected(EndpointConnectionManager endpointConnectionManager,
				ServerEndpoint serverEndpoint)
			{
				EndpointConnectionManager = endpointConnectionManager;
				ServerEndpoint = serverEndpoint;
			}

			public EndpointConnectionManager EndpointConnectionManager { get; }
			public ServerEndpoint ServerEndpoint { get; }
		}

		public class Disconnected
		{
			public Disconnected(EndpointConnectionManager endpointConnectionManager,
				ServerEndpoint serverEndpoint)
			{
				EndpointConnectionManager = endpointConnectionManager;
				ServerEndpoint = serverEndpoint;
			}

			public EndpointConnectionManager EndpointConnectionManager { get; }
			public ServerEndpoint ServerEndpoint { get; }
		}
	}
}
