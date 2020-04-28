using HomeCtl.Connection;
using HomeCtl.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{
	public static class GenericHostBuilderExtensions
	{
		public static IHostBuilder ConfigurHomeCtlHostDefaults(this IHostBuilder builder, Action<IWebHostBuilder> configure)
		{
			return builder.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.ConfigureServices(svcs =>
				{
					svcs.AddSingleton<ServerConnector>();
					svcs.AddGrpc();
					svcs.AddHostedService<HomeCtlHostService>();
				});
				webBuilder.Configure(appBuilder =>
				{
					appBuilder.UseRouting();
					appBuilder.UseEndpoints(endpoints =>
					{
						endpoints.MapGrpcService<HomeCtl.Host.ProtocolServices.HostInterfaceService>();
					});
				});
				configure?.Invoke(webBuilder);
			});
		}
	}
}
