using HomeCtl.Connection;
using HomeCtl.Kinds;
using System.Collections.Generic;

namespace HomeCtl.ApiServer.Hosts
{
	class HostEndpointConnectionFactory : IConnectionProviderFactory
	{
		public IEnumerable<IConnectionProvider> CreateConnectionProviders(Host host)
		{
			yield return new EndpointPropertyConnectionProvider(host);
		}
	}
}
