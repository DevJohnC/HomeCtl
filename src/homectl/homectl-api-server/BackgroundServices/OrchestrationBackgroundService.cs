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

		public OrchestrationBackgroundService(
			OrchestrationConductor orchestrationConductor
			)
		{
			_orchestrationConductor = orchestrationConductor;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return _orchestrationConductor.Run(stoppingToken);
		}
	}
}
