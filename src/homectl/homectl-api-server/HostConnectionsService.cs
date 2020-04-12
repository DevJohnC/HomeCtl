using HomeCtl.ApiServer.Hosts;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer
{
	/// <summary>
	/// Entry point for ensuring that host connections are running.
	/// </summary>
	class HostConnectionsService : BackgroundService
	{
		private readonly HostsConnectionManager _hostsConnectionManager;

		public HostConnectionsService(Hosts.HostsConnectionManager hostsConnectionManager)
		{
			_hostsConnectionManager = hostsConnectionManager;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return _hostsConnectionManager.Run(stoppingToken);
		}
	}
}
