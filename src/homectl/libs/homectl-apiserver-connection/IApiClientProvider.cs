using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public interface IApiClientProvider
	{
		Task<HttpClient> CreateApiClient(CancellationToken stoppingToken);
	}
}
