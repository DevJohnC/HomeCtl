using HomeCtl.Connection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Host
{
	class HomeCtlHostService : BackgroundService
	{
		private readonly EndpointConnectionManager _connectionManager;
		private readonly IEnumerable<IServerEndpointProvider> _serverEndpointProviders;
		private readonly IEnumerable<IServerLivelinessMonitor> _livelinessMonitors;

		public HomeCtlHostService(
			EndpointConnectionManager connectionManager,
			IEnumerable<IServerEndpointProvider> serverEndpointProviders,
			IEnumerable<IServerLivelinessMonitor> livelinessMonitors,
			IEnumerable<StartupService> startupServices //  ensure startup services are constructed before this service runs
			)
		{
			_connectionManager = connectionManager;
			_serverEndpointProviders = serverEndpointProviders;
			_livelinessMonitors = livelinessMonitors;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return _connectionManager.Run(_serverEndpointProviders, _livelinessMonitors, stoppingToken);
		}
	}
}
