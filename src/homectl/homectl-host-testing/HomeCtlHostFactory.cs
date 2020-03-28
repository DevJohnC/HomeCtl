using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace homectl.Testing
{
	public abstract class HomeCtlHostFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
		where TEntryPoint : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureServices(services => services.AddHomeCtl());
			builder.Configure(builder => builder.UseHomeCtl());
		}
	}
}
