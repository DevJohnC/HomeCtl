using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Monitors if a server is available or not.
	/// </summary>
	public interface IServerLivelinessMonitor
	{
		Task MonitorForDisconnect(CancellationToken stoppingToken);
	}
}
