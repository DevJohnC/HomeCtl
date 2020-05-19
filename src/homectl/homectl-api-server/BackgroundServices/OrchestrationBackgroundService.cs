using HomeCtl.ApiServer.Orchestration;
using HomeCtl.ApiServer.Resources;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.BackgroundServices
{
	/// <summary>
	/// Runs the OrchestrationConductor.
	/// </summary>
	class OrchestrationBackgroundService : BackgroundService
	{
		private readonly OrchestrationConductor _orchestrationConductor;
		private readonly ResourceOrchestrator _resourceOrchestrator;

		public OrchestrationBackgroundService(
			OrchestrationConductor orchestrationConductor,
			ResourceOrchestrator resourceOrchestrator
			)
		{
			_orchestrationConductor = orchestrationConductor;
			_resourceOrchestrator = resourceOrchestrator;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _resourceOrchestrator.LoadResources();
			await _orchestrationConductor.Run(stoppingToken);
		}
	}
}
