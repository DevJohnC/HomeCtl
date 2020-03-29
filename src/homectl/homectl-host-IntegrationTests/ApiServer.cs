using homectl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace homectl_host_IntegrationTests
{
	class ApiServer : IDisposable
	{
		private readonly IHost _host;

		public ApiServer()
		{
			_host = CreateHost();
			_host.Start();
		}

		private IHost CreateHost()
			=> Host.CreateDefaultBuilder()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseUrls("http://localhost:16674/");
					webBuilder.UseKestrel(options =>
					{
						options.ConfigureEndpointDefaults(listenOpts =>
						{
							listenOpts.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
						});
					});
					webBuilder.UseStartup<Startup>();
				}).Build();

		public HttpClient CreateClient()
		{
			AppContext.SetSwitch(
				"System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
			return new HttpClient
			{
				BaseAddress = new Uri("http://localhost:16674/"),
				DefaultRequestVersion = new Version(2, 0)
			};
		}

		public void Dispose()
		{
			_host.Dispose();
		}
	}
}
