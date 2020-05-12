using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace homectl_server_connection_tests
{
	public static class IHostTestExtensions
	{
		private const string SWITCH_NAME = "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport";

		public static HttpClient CreateFixedClient(this IHost host)
		{
			AppContext.TryGetSwitch(SWITCH_NAME, out var currentSwitchValue);
			if (currentSwitchValue == false)
				AppContext.SetSwitch(SWITCH_NAME, true);

			// Need to set the response version to 2.0.
			// Required because of this TestServer issue - https://github.com/aspnet/AspNetCore/issues/16940
			var responseVersionHandler = new ResponseVersionHandler();
			responseVersionHandler.InnerHandler = host.GetTestServer().CreateHandler();

			var client = new HttpClient(responseVersionHandler)
			{
				BaseAddress = new Uri("http://localhost"),
				DefaultRequestVersion = new Version(2, 0)
			};

			if (currentSwitchValue == false)
				AppContext.SetSwitch(SWITCH_NAME, false);

			return client;
		}

		private class ResponseVersionHandler : DelegatingHandler
		{
			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				var response = await base.SendAsync(request, cancellationToken);
				response.Version = request.Version;

				return response;
			}
		}
	}
}
