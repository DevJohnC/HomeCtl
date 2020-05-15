using HomeCtl.Connection;
using HomeCtl.Events;
using HomeCtl.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Hosting
{
	public static class GenericHostBuilderExtensions
	{
		public static IHostBuilder ConfigureHomeCtlHostDefaults(this IHostBuilder builder)
		{
			return builder
				.ConfigureWebHostDefaults(webBuilder => webBuilder.ConfigureHomeCtlHostDefaults());
		}

		private static IWebHostBuilder ConfigureHomeCtlHostDefaults(this IWebHostBuilder builder)
		{
			var randomizedPort = PortRandomizer.GetRandomPort();

			builder.ConfigureKestrel(options =>
			{
				options.ListenAnyIP(randomizedPort);
			});

			builder.ConfigureServices(svcs =>
			{
				svcs.Configure<HomeCtl.Host.HostOptions>(options => {
					options.HostFile = "host.json";
					options.HostPort = randomizedPort;
				});

				svcs.AddStartupService<HostRecordsService>();
				svcs.AddStartupService<AppHost>(sP =>
					sP.GetRequiredService<IOptions<HomeCtl.Host.HostOptions>>().Value.GetAppHost());

				svcs.AddSingleton<EventBus>();
				svcs.AddGrpc();
				svcs.AddHostedService<HomeCtlHostService>();
				svcs.AddSingleton<ApiServer>();
				svcs.AddSingleton<EndpointConnectionManager>();
				svcs.AddSingleton<IEndpointClientFactory>(sP =>
					new ReuseSingleClientFactory(new System.Net.Http.HttpClient()));
				svcs.AddSingleton<IServerIdentityVerifier, GrpcIdentityVerifier>();
				svcs.AddSingleton<IServerLivelinessMonitor, NetworkErrorLivelinessMonitor>();
			});

			builder.Configure(appBuilder =>
			{
				appBuilder.UseRouting();
				appBuilder.UseEndpoints(endpoints =>
				{
					endpoints.MapGrpcService<HomeCtl.Host.ProtocolServices.HostInterfaceService>();
				});
			});

			return builder;
		}
	}
}
