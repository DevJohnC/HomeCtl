using HomeCtl.Host;
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
					svcs.AddHostedService<HomeCtlHostService>();
				});
				configure?.Invoke(webBuilder);
			});
		}
	}
}
