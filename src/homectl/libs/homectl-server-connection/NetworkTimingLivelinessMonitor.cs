using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public class NetworkTimingLivelinessMonitor : IServerLivelinessMonitor
	{
		private Server _server;

		public NetworkTimingLivelinessMonitor(Server server)
		{
			_server = server;
		}

		public async Task MonitorForDisconnect(ServerEndpoint serverEndpoint, CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await _server.GetNetworkTiming();
					await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
				}
				catch
				{
					return;
				}
			}
		}
	}
}
