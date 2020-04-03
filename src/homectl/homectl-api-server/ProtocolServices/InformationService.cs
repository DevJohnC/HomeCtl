using Grpc.Core;
using HomeCtl.Servers.ApiServer;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class InformationService : Information.InformationBase
	{
		public override Task<IpAddressResponse> GetClientIpAddress(Empty request, ServerCallContext context)
		{
			return Task.FromResult(
				new IpAddressResponse
				{
					IpAddress = context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? ""
				});
		}
	}
}
