using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace homectl
{
	public abstract class ApiServerEndpoint
	{
		public abstract Task<ConnectionResult> AttemptConnection(CancellationToken cancellationToken);

		public struct ConnectionResult
		{
			public ConnectionResult(bool success, HttpClient? httpClient)
			{
				Success = success;
				HttpClient = httpClient;
			}

			public bool Success { get; }

			public HttpClient? HttpClient { get; }


		}
	}

	public class ConfiguredHttpClientEndpoint : ApiServerEndpoint
	{
		public ConfiguredHttpClientEndpoint(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public HttpClient HttpClient { get; }

		public override Task<ConnectionResult> AttemptConnection(CancellationToken cancellationToken)
		{
			return Task.FromResult(new ConnectionResult(true, HttpClient));
		}
	}
}
