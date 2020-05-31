using HomeCtl.ApiServer.Orchestration;
using HomeCtl.ApiServer.Resources;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
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
		private readonly ResourceStateStore _resourceOrchestrator;

		public OrchestrationBackgroundService(
			OrchestrationConductor orchestrationConductor,
			ResourceStateStore resourceOrchestrator,
			IEnumerable<StartupService> startupServices
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
