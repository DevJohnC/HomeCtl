using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public class GrpcIdentityVerifier : IServerIdentityVerifier
	{
		public Task<bool> VerifyServer(ServerEndpoint serverEndpoint, HttpClient httpClient, CancellationToken stoppingToken)
		{
			//  todo: query server for identity credentials
			return Task.FromResult(serverEndpoint.IdentityPolicy.IsValid(
				default
				));
		}
	}
}
