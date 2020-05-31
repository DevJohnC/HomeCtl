using HomeCtl.ApiServer.Resources;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Hosts
{
	static class HostExtensions
	{
		public static Task SetConnectedState(this Host host, ResourceStateManager resourceStateManager, Host.ConnectedState connectedState)
		{
			return resourceStateManager.PatchState(host,
				new HomeCtl.Kinds.Resources.ResourceState(new List<ResourceField>
				{
					new ResourceField("connectedState", ResourceFieldValue.String(connectedState.ToString()))
				}));
		}
	}
}
