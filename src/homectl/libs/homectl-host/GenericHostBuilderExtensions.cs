using HomeCtl.Connection;
using HomeCtl.Events;
using HomeCtl.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.Hosting
{
	public static class GenericHostBuilderExtensions
	{
		public static IHostBuilder ConfigurHomeCtlHostDefaults(this IHostBuilder builder, Action<IWebHostBuilder> configure)
		{
			return builder.ConfigureWebHostDefaults(webBuilder => webBuilder.ConfigurHomeCtlHostDefaults(configure));
		}

		public static IWebHostBuilder ConfigurHomeCtlHostDefaults(this IWebHostBuilder builder, Action<IWebHostBuilder> configure)
		{
			configure?.Invoke(builder);
			builder.ConfigureServices(svcs =>
			{
				svcs.Configure<HomeCtl.Host.HostOptions>(options => {
					options.HostFile = "host.json";
				});

				svcs.AddSingleton<AppHost>(sP => sP.GetRequiredService<IOptions<HomeCtl.Host.HostOptions>>().Value.GetAppHost());
				svcs.AddSingleton<EventBus>();
				svcs.AddGrpc();
				svcs.AddHostedService<HomeCtlHostService>();
				svcs.AddSingleton<ApiServer>();
				svcs.AddSingleton<EndpointConnectionManager>();
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
