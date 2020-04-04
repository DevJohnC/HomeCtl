using HomeCtl.Connection;
using HomeCtl.Kinds;
using System;

namespace HomeCtl.ApiServer.Hosts
{
	public class ManagedHost
	{
		public ManagedHost(Guid id, Host host)
		{
			Id = id;
			Host = host;
		}

		public Guid Id { get; }

		public Host Host { get; }

		public ConnectionManager? ConnectionManager { get; }
	}
}
