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
		public static IApplicationBuilder UseHomeCtlHost(this IApplicationBuilder applicationBuilder)
		{
			applicationBuilder.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<HomeCtl.Host.ProtocolServices.HostInterfaceService>();
				endpoints.MapGrpcService<HomeCtl.Host.ProtocolServices.InformationService>();
				endpoints.MapGrpcService<HomeCtl.Host.ProtocolServices.NetworkService>();
			});
			return applicationBuilder;
		}

		public static IWebHostBuilder ConfigureHomeCtlHostDefaults(this IWebHostBuilder builder)
		{
			var randomizedPort = PortRandomizer.GetRandomPort();

			builder.ConfigureKestrel(options =>
			{
				options.ListenAnyIP(randomizedPort, listenOptions =>
				{
					listenOptions.Protocols = AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
				});
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
					new ReuseSingleClientFactory(new System.Net.Http.HttpClient { DefaultRequestVersion = new System.Version(2,0) }));
				svcs.AddSingleton<IServerIdentityVerifier, GrpcIdentityVerifier>();
				svcs.AddSingleton<IServerLivelinessMonitor, NetworkErrorLivelinessMonitor>();
				svcs.AddSingleton<IServerLivelinessMonitor>(sP => new NetworkTimingLivelinessMonitor(sP.GetRequiredService<ApiServer>()));
			});

			return builder;
		}
	}
}
