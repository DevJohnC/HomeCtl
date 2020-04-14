using HomeCtl.ApiServer.ProtocolServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace HomeCtl.ApiServer
{
	class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<Newtonsoft.Json.JsonSerializer>(new Newtonsoft.Json.JsonSerializer());

			services.AddSingleton<Orchestration.ISpecApplierFactory, Orchestration.NetworkedSpecApplierFactory>();
			services.AddSingleton<Orchestration.OrchestrationConductor>();
			services.AddHostedService<BackgroundServices.OrchestrationBackgroundService>();

			services.AddSingleton<Resources.ResourceManager>();
			services.AddSingleton<Resources.ResourceSegmentManager>();

			services.AddSingleton<Hosts.HostsManager>();
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
