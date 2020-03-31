using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace homectl_api_server_IntegrationTests
{
	[TestClass]
	public class MyRandomTests
	{
		[TestMethod]
		public async Task MyJsonTest()
		{
			using (var webApp = new TestApplicationFactory())
			using (var client = webApp.CreateClient())
			{
				var content = new StringContent(@"{
	metadata: {
		hostname: ""test-host""
	},
	spec: {
		endpoint: ""http://localhost/""
	}
}", Encoding.UTF8, "application/json");
				var response = await client.PutAsync("/apis/core/v1alpha1/host/FEED0527-5081-4440-A318-D30E1FECBA91", content);

				response.EnsureSuccessStatusCode();

				var fullData = await response.Content.ReadAsStringAsync();
			}
		}
	}
}
