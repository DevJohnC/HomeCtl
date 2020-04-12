using HomeCtl.ApiServer.ProtocolServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeCtl.ApiServer
{
	class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHostedService<HostConnectionsService>();
			services.AddSingleton<Hosts.HostsConnectionManager>();
			services.AddSingleton<Hosts.HostsManager>();
			services.AddSingleton<Hosts.IConnectionProviderFactory, Hosts.HostEndpointConnectionFactory>();
			services.AddGrpc();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<InformationService>();
				endpoints.MapGrpcService<HostsService>();
			});
		}
	}
}
