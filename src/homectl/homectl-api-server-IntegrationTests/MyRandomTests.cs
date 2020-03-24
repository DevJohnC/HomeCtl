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
		id: ""FBEF67E3-6AFA-4345-9286-69FA24FA9862""
	},
	spec: {
		hostname: ""my-hostname""
	}
}", Encoding.UTF8, "application/json");
				var response = await client.PostAsync("/apis/core/v1alpha1/node", content);

				response.EnsureSuccessStatusCode();
			}
		}
	}
}
