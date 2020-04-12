using HomeCtl.Connection;
using HomeCtl.Kinds;
using System.Collections.Generic;

namespace HomeCtl.ApiServer.Hosts
{
	/// <summary>
	/// Creates connection providers for connecting to hosts.
	/// </summary>
	public interface IConnectionProviderFactory
	{
		/// <summary>
		/// Create a connection provider to connect to the specified host.
		/// </summary>
		/// <param name="host"></param>
		/// <returns></returns>
		IEnumerable<IConnectionProvider> CreateConnectionProviders(Host host);
	}
}
