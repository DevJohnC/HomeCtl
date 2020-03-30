using System.Net.Http;
using System.Threading.Tasks;

namespace homectl
{
	public class ResourceClient<TResource>
	{
		public ResourceClient(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public HttpClient HttpClient { get; }

		public async Task Save(TResource resource)
		{

		}
	}
}
