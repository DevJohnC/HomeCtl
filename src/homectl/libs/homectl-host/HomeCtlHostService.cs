using HomeCtl.Connection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Host
{
	class HomeCtlHostService : BackgroundService
	{
		private readonly ServerConnector _serverConnector;

		public HomeCtlHostService(ServerConnector serverConnector)
		{
			_serverConnector = serverConnector;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return _serverConnector.Run(stoppingToken);
		}
	}
}
