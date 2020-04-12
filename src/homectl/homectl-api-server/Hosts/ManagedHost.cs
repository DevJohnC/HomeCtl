using HomeCtl.Connection;
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
			ConnectionManager = default;
		}

		public Guid Id { get; }

		public Host Host { get; }

		public ConnectionManager? ConnectionManager { get; }

		public ManagedHost WithHost(Host host)
		{
			return new ManagedHost(Id, host);
		}
	}
}
