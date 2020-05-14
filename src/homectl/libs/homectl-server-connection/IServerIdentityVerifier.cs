using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public interface IServerIdentityVerifier
	{
		Task<bool> VerifyServer(ServerEndpoint serverEndpoint, HttpClient httpClient, CancellationToken stoppingToken);
	}
}
