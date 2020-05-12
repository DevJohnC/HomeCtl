using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public interface IServerEndpointProvider
	{
		Task<ServerEndpoint> GetServerEndpoint(CancellationToken stoppingToken);
	}
}
