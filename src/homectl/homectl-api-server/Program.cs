using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace homectl
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseKestrel(options =>
					{
						options.ConfigureEndpointDefaults(listenOpts =>
						{
							listenOpts.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
						});
					});
					webBuilder.UseStartup<Startup>();
				});
	}
}
