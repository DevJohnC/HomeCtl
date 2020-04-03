using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Provides a connection via a pre-configured HttpClient instance.
	/// </summary>
	public class ConfiguredHttpClientProvider : IConnectionProvider
	{
		private readonly HttpClient _httpClient;

		public ConfiguredHttpClientProvider(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public Task<ConnectionResult> AttemptConnection(CancellationToken cancellationToken)
		{
			return Task.FromResult(new ConnectionResult(true, _httpClient));
		}
	}
}
