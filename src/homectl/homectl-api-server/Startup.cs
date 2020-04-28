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
			services.AddSingleton<Events.EventBus>();

			services.AddSingleton<Orchestration.OrchestrationConductor>();
			services.AddHostedService<BackgroundServices.OrchestrationBackgroundService>();

			services.AddSingleton<Resources.ResourceStore>();

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
				endpoints.MapGrpcService<RecordsService>();
				endpoints.MapGrpcService<ResourcesService>();
			});
		}
	}
}
