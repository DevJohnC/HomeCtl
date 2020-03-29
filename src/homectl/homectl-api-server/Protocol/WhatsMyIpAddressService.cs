using Grpc.Core;
using System.Threading.Tasks;

namespace homectl.Protocol
{
	public class WhatsMyIpAddressService : WhatsMyIpAddress.WhatsMyIpAddressBase
	{
		public override Task<IpAddressResponse> GetIpAddress(IpAddressRequest request, ServerCallContext context)
		{
			return Task.FromResult(
				new IpAddressResponse { IpAddress = context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "" }
				);
		}
	}
}
