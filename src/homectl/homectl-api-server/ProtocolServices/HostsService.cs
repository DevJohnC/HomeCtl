using Grpc.Core;
using HomeCtl.ApiServer.Hosts;
using HomeCtl.Servers.ApiServer;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class HostsService : Servers.ApiServer.Hosts.HostsBase
	{
		private readonly HostsManager _hostsManager;

		public HostsService(HostsManager hostsManager)
		{
			_hostsManager = hostsManager;
		}

		public override Task<Empty> Store(HostStoreRequest request, ServerCallContext context)
		{
			var remoteHostName = context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "localhost";
			var managedHosts = _hostsManager.FindMatchingHosts(request.HostMatchQuery);
			if (managedHosts.Count == 0)
			{
				managedHosts = new ManagedHost[]
				{
					//  todo: create managed host instance
				};
			}

			//  todo: call an update method on _hostsManager

			return Task.FromResult(Empty.Instance);
		}
	}
}
