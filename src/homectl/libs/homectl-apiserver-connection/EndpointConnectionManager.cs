using Grpc.Core;
using HomeCtl.Events;
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

		public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.NotConnected;
		public ServerEndpoint Endpoint { get; private set; }
		public ChannelBase? ServicesChannel { get; private set; }

		public EndpointConnectionManager(EventBus eventBus,
			IEndpointClientFactory endpointClientFactory,
			IServerIdentityVerifier serverIdentifyVerifier)
		{
			EventBus = eventBus;
			_endpointClientFactory = endpointClientFactory;
			_serverIdentifyVerifier = serverIdentifyVerifier;
		}

		private void SetDisconnectedState()
		{
			var previousStatus = ConnectionStatus;
			ConnectionStatus = ConnectionStatus.NotConnected;

			if (previousStatus == ConnectionStatus.Connected)
			{
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
				EventBus.Publish(
					new EndpointConnectionEvents.Connected(this, Endpoint)
					);
			}
		}

		public async Task Run(IEnumerable<IServerEndpointProvider> endpointProviders,
			IEnumerable<IServerLivelinessMonitor> livelinessMonitors,
			CancellationToken stoppingToken)
		{
			var endpointProviderArray = endpointProviders?.ToArray();
			var livelinessMonitorsArray = livelinessMonitors?.ToArray();
			if (endpointProviderArray == null || endpointProviderArray.Length == 0)
				throw new Exception("Impossible to locate server, provide 1 or more endpoint providers.");
			if (livelinessMonitorsArray == null || livelinessMonitorsArray.Length == 0)
				throw new Exception("Impossible to monitor server connection, provide 1 or more liveliness monitors.");

			while (!stoppingToken.IsCancellationRequested)
			{
				switch (ConnectionStatus)
				{
					case ConnectionStatus.NotConnected:
						await ConnectToServer(endpointProviderArray, stoppingToken);
						break;
					case ConnectionStatus.Connected:
						await WaitForDisconnect(livelinessMonitorsArray, stoppingToken);
						break;
				}
			}
		}

		private async Task ConnectToServer(IServerEndpointProvider[] endpointProviders, CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await AttemptNextConnection(endpointProviders, stoppingToken);

				if (ConnectionStatus == ConnectionStatus.Connected)
					return;

				if (!stoppingToken.IsCancellationRequested)
					await Task.Delay(TimeSpan.FromSeconds(5));
			}
		}

		private async Task AttemptNextConnection(IServerEndpointProvider[] endpointProviders, CancellationToken stoppingToken)
		{
			try
			{
				//  get next endpoint from provider
				var (serverEndpoint, endpointProvider) = await GetNextEndpoint(endpointProviders, stoppingToken);
				//  create client
				var httpClient = _endpointClientFactory.CreateHttpClient(serverEndpoint);
				//  attempt to verify
				if (!(await _serverIdentifyVerifier.VerifyServer(serverEndpoint, httpClient)))
					return;
				//  setup connection
				Endpoint = serverEndpoint;
				ServicesChannel = Grpc.Net.Client.GrpcChannel.ForAddress(
					serverEndpoint.Uri, new Grpc.Net.Client.GrpcChannelOptions
					{
						DisposeHttpClient = false,
						HttpClient = httpClient
					});
				//  set status
				SetConnectedState();
			}
			catch (Exception ex)
			{
				//  todo: log exception
			}
		}

		private async Task<(ServerEndpoint ServerEndpoint, IServerEndpointProvider EndpointProvider)> GetNextEndpoint(
			IServerEndpointProvider[] endpointProviders, CancellationToken stoppingToken
			)
		{
			using (var stopConnectingToken = new CancellationTokenSource())
			{
				var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
					stoppingToken, stopConnectingToken.Token
					);

				var endpointResolveTasks = endpointProviders
					.Select(q => GetEndpoint(q, cancellationSource.Token))
					.ToList();

				var finishedTask = await Task.WhenAny(endpointResolveTasks);

				var result = await finishedTask;

				stopConnectingToken.Cancel();

				return result;
			}
		}

		private async Task<(ServerEndpoint ServerEndpoint, IServerEndpointProvider EndpointProvider)> GetEndpoint(IServerEndpointProvider endpointProvider, CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var endpoint = await endpointProvider.GetServerEndpoint(stoppingToken);
					return (endpoint, endpointProvider);
				}
				catch (Exception ex)
				{
					//  todo: log exception
				}
			}

			return (default, endpointProvider);
		}

		private async Task WaitForDisconnect(IServerLivelinessMonitor[] livelinessMonitors, CancellationToken stoppingToken)
		{
			using (var stopMonitoringSource = new CancellationTokenSource())
			{
				var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
					stoppingToken, stopMonitoringSource.Token
					);

				//  wait for a monitor to complete or throw an exception
				var monitorTasks = livelinessMonitors
					.Select(q => RunLivelinessMonitor(q, cancellationSource.Token))
					.ToList();

				await Task.WhenAny(monitorTasks);

				//  destroy connection
				Endpoint = default;
				await (ServicesChannel?.ShutdownAsync() ?? Task.CompletedTask);
				ServicesChannel = null;

				//  set state
				SetDisconnectedState();

				stopMonitoringSource.Cancel();
			}
		}

		private async Task RunLivelinessMonitor(IServerLivelinessMonitor livelinessMonitor, CancellationToken stoppingToken)
		{
			try
			{
				await livelinessMonitor.MonitorForDisconnect(stoppingToken);
			}
			catch (Exception ex)
			{
				//  todo: log exception
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
