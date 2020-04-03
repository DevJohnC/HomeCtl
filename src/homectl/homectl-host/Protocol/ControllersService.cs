using Grpc.Core;
using homectl.Protocol.Server.Host;
using System.Threading.Tasks;

namespace homectl.Protocol
{
	public class ControllersService : Server.Host.Controllers.ControllersBase
	{
		public override Task<ApplyDeviceSpecResponse> ApplyDeviceSpec(ApplyDeviceSpecRequest request, ServerCallContext context)
		{
			return base.ApplyDeviceSpec(request, context);
		}
	}
}
