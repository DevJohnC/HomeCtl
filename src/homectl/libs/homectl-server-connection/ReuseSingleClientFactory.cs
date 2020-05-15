using System.Net.Http;

namespace HomeCtl.Connection
{
	public class ReuseSingleClientFactory : IEndpointClientFactory
	{
		private readonly HttpClient _httpClient;

		public ReuseSingleClientFactory(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public HttpClient CreateHttpClient(ServerEndpoint serverEndpoint)
		{
			return _httpClient;
		}
	}
}
