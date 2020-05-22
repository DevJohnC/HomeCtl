using HomeCtl.ApiServer.Hosts;
using HomeCtl.Connection;
using HomeCtl.Events;
using HomeCtl.Kinds;
using HomeCtl.Services.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Connections
{
	class ConnectionManagerHostestService : BackgroundService
	{
		private readonly ConnectionManager _connectionManager;

		public ConnectionManagerHostestService(ConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
			=> _connectionManager.Run(stoppingToken);
	}

	class ConnectionManager
	{
		private readonly Dictionary<Guid, EndpointConnectionRunner> _hostConnections =
			new Dictionary<Guid, EndpointConnectionRunner>();
		private readonly EventBus _eventBus;
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<ConnectionManager> _logger;

		public ConnectionManager(EventBus eventBus, ILoggerFactory loggerFactory)
		{
			_eventBus = eventBus;
			_loggerFactory = loggerFactory;
			_logger = loggerFactory.CreateLogger<ConnectionManager>();
		}

		public void CreateConnection(HostServer host)
		{
			_hostConnections.Add(
				host.Host.HostId,
				new EndpointConnectionRunner(
					host,
					host.ConnectionManager,
					_logger,
					_eventBus,
					_loggerFactory));
		}

		public void UpdateConnection(HostServer host)
		{
			if (!_hostConnections.TryGetValue(host.Host.HostId, out var runner))
				return;

			runner.EndpointProvider.HostUri = new Uri(host.Host.Endpoint);
		}

		private async Task<EndpointConnectionRunner?> Delay(TimeSpan timeSpan)
		{
			try
			{
				await Task.Delay(timeSpan);
			}
			//  prevent throwing an exception for being cancelled
			catch { }

			return null;
		}

		public async Task Run(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var finishedTask = await Task.WhenAny(
					_hostConnections.Values.Select(q => q.WaitForFinish(stoppingToken))
						.Concat(new[] { Delay(TimeSpan.FromSeconds(1)) })
					);

				var runner = await finishedTask;

				if (runner == null) // delay task
					continue;

				_hostConnections.Remove(runner.Host.HostId);
			}
		}

		private class EndpointConnectionRunner
		{
			public HomeCtl.Kinds.Host Host => HostServer.Host;

			public HostServerEndpointProvider EndpointProvider { get; }
			public HostServer HostServer { get; }

			private readonly EndpointConnectionManager _connectionManager;
			private readonly ILogger<ConnectionManager> _logger;
			private readonly EventBus _eventBus;
			private readonly ILoggerFactory _loggerFactory;
			private Task<EndpointConnectionRunner?>? _runningTask;

			public EndpointConnectionRunner(
				HostServer host,
				EndpointConnectionManager connectionManager,
				ILogger<ConnectionManager> logger,
				EventBus eventBus,
				ILoggerFactory loggerFactory)
			{
				HostServer = host;
				_connectionManager = connectionManager;
				_logger = logger;
				_eventBus = eventBus;
				_loggerFactory = loggerFactory;
				EndpointProvider = new HostServerEndpointProvider(new Uri(Host.Endpoint));
			}

			private async Task<EndpointConnectionRunner?> Run(CancellationToken stoppingToken)
			{
				try
				{
					await _connectionManager.Run(
							new[] { EndpointProvider },
							new IServerLivelinessMonitor[]
							{
								new NetworkErrorLivelinessMonitor(_eventBus,
									_loggerFactory.CreateLogger<NetworkErrorLivelinessMonitor>()),
								new NetworkTimingLivelinessMonitor(HostServer)
							},
							stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Encountered an exception while running endpoint connection.");
				}

				return this;
			}

			public Task<EndpointConnectionRunner?> WaitForFinish(CancellationToken stoppingToken)
			{
				if (_runningTask == null)
					_runningTask = Run(stoppingToken);
				return _runningTask;
			}
		}

		private class HostServerEndpointProvider : IServerEndpointProvider
		{
			public HostServerEndpointProvider(Uri hostUri)
			{
				HostUri = hostUri;
			}

			public Uri HostUri { get; set; }

			public Task<ServerEndpoint> GetServerEndpoint(CancellationToken stoppingToken)
			{
				return Task.FromResult(new ServerEndpoint(HostUri, AnyServerIdentityPolicy.Instance));
			}
		}
	}
}
