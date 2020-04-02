using homectl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

namespace Microsoft.Extensions.Hosting
{
	public static class GenericHostBuilderExtensions
	{
		public static IHostBuilder ConfigureCtlHostDefaults(this IHostBuilder builder)
		{
			return builder.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder
					.UseKestrel(options =>
					{
						options.ListenLocalhost(27891, o => o.Protocols =
							HttpProtocols.Http2);
						options.ConfigureEndpointDefaults(listenOpts =>
						{
							listenOpts.Protocols = HttpProtocols.Http2;
						});
					})
					.ConfigureServices(services => services.AddHomeCtl())
					.Configure(builder => builder.UseHomeCtl());
			});
		}

		public static IServiceCollection AddHomeCtl(this IServiceCollection services)
		{
			services.AddSingleton<ConnectionManager>(sP => 
					new ConnectionManager(sP.GetRequiredService<IEnumerable<ApiServerEndpoint>>())
				);
			services.AddHostedService<HomeCtlHost>();
			services.AddGrpc();
			return services;
		}

		public static IApplicationBuilder UseHomeCtl(this IApplicationBuilder builder)
		{
			builder.UseRouting();
			builder.UseEndpoints(endpoints =>
			{
			});
			return builder;
		}
	}
}
