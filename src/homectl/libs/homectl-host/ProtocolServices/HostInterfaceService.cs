using Grpc.Core;
using HomeCtl.Servers.Host;
using System.Threading.Tasks;

namespace HomeCtl.Host.ProtocolServices
{
	class HostInterfaceService : HostInterface.HostInterfaceBase
	{
		public override Task<ProcessIntentResponse> ProcessIntent(ProcessIntentRequest request, ServerCallContext context)
		{
			return base.ProcessIntent(request, context);
		}
	}
}
