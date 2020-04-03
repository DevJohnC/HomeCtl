using Grpc.Core;
using homectl.Protocol.Client;
using System.Threading.Tasks;

namespace homectl.Protocol
{
	public class InformationService : Information.InformationBase
	{
		public override Task<IpAddressResponse> GetClientIpAddress(Empty request, ServerCallContext context)
		{
			return Task.FromResult(
				new IpAddressResponse { IpAddress = context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "" }
				);
		}
	}
}
