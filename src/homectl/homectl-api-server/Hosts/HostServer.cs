using HomeCtl.Connection;
using HomeCtl.Events;
using HomeCtl.Kinds;
using HomeCtl.Services;
using HomeCtl.Services.Server;
using Microsoft.Extensions.Logging;
using System;

namespace HomeCtl.ApiServer.Hosts
{
	class HostServer : Server
	{
		public HostServer(Host host, 
			EndpointConnectionManager connectionManager, EventBus eventBus,
			ILogger<HostServer> logger) :
			base(connectionManager, eventBus, logger)
		{
			Host = host;
		}

		public Host Host { get; }
	}
}
