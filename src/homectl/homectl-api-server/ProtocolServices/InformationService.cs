using Grpc.Core;
using HomeCtl.Servers.ApiServer;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class InformationService : Information.InformationBase
	{
		private static readonly System.Version AssemblyVersion = typeof(InformationService).Assembly.GetName().Version ?? new System.Version(0, 0);

		public override Task<IpAddressResponse> GetClientIpAddress(Empty request, ServerCallContext context)
		{
			return Task.FromResult(new IpAddressResponse
			{
				IpAddress = context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? ""
			});
		}

		public override Task<ServerVersionResponse> GetServerVersion(Empty request, ServerCallContext context)
		{
			return Task.FromResult(new ServerVersionResponse
			{
				ApiServerVersion = new Version
				{
					Major = AssemblyVersion.Major,
					Minor = AssemblyVersion.Minor,
					Name = "automata-indev"
				}
			});
		}
	}
}
