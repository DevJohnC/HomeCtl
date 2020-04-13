using HomeCtl.Kinds;
using System;

namespace HomeCtl.ApiServer.Hosts
{
	public struct ManagedHost
	{
		public ManagedHost(Guid id, Host host)
		{
			Id = id;
			Host = host;
		}

		public Guid Id { get; }

		public Host Host { get; }

		public ManagedHost WithHost(Host host)
		{
			return new ManagedHost(Id, host);
		}
	}
}
