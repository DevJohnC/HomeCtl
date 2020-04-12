using HomeCtl.Connection;
using HomeCtl.Kinds;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Hosts
{
	public class EndpointPropertyConnectionProvider : IConnectionProvider
	{
		private readonly string _endpoint;

		public EndpointPropertyConnectionProvider(Host host)
		{
			_endpoint = host.State.Endpoint;
		}

		public Task<ConnectionResult> AttemptConnection(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
