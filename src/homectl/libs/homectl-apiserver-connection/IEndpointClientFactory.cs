using System.Net.Http;

namespace HomeCtl.Connection
{
	public interface IEndpointClientFactory
	{
		HttpClient CreateHttpClient(ServerEndpoint serverEndpoint);
	}
}
