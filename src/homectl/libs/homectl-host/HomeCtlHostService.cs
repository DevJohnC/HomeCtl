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

		public HomeCtlHostService(
			EndpointConnectionManager connectionManager,
			IEnumerable<IServerEndpointProvider> serverEndpointProviders,
			ApiServer apiServer //  injected to ensure ApiServer has hooked into events
			)
		{
			_connectionManager = connectionManager;
			_serverEndpointProviders = serverEndpointProviders;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return _connectionManager.Run(_serverEndpointProviders, stoppingToken);
		}
	}
}
