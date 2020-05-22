using HomeCtl.ApiServer.Connections;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Connection;
using HomeCtl.Events;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Hosts
{
	class HostManager : ResourceManager<Host>
	{
		private readonly ConnectionManager _connectionManager;
		private readonly EventBus _eventBus;
		private readonly ILoggerFactory _loggerFactory;
		private readonly IEndpointClientFactory _endpointClientFactory;
		private readonly IServerIdentityVerifier _serverIdentityVerifier;

		private readonly object _lock = new object();
		private readonly Dictionary<Guid, HostServer> _hosts =
			new Dictionary<Guid, HostServer>();

		protected override Kind<Host> TypedKind => CoreKinds.Host;

		public HostManager(ConnectionManager connectionManager, EventBus eventBus,
			ILoggerFactory loggerFactory, IEndpointClientFactory endpointClientFactory,
			IServerIdentityVerifier serverIdentityVerifier, IResourceDocumentStore<Host> documentStore) :
			base(documentStore)
		{
			_connectionManager = connectionManager;
			_eventBus = eventBus;
			_loggerFactory = loggerFactory;
			_endpointClientFactory = endpointClientFactory;
			_serverIdentityVerifier = serverIdentityVerifier;
		}

		protected override Task OnResourceCreated(Host resource)
		{
			var hostServer = new HostServer(resource,
				new EndpointConnectionManager(
					_eventBus, _endpointClientFactory, _serverIdentityVerifier,
					_loggerFactory.CreateLogger<EndpointConnectionManager>()
					),
				_eventBus,
				_loggerFactory.CreateLogger<HostServer>(),
				this
				);

			lock (_lock)
			{
				_hosts.Add(resource.HostId, hostServer);
			}

			_connectionManager.CreateConnection(hostServer);
			return base.OnResourceCreated(resource);
		}

		protected override async Task OnResourceUpdated(Host newResource, Host oldResource)
		{
			if (!_hosts.TryGetValue(newResource.HostId, out var host))
				return;

			oldResource.Endpoint = newResource.Endpoint;
			oldResource.MachineName = newResource.MachineName;
			_connectionManager.UpdateConnection(host);

			await base.OnResourceUpdated(newResource, oldResource);
		}

		protected override async Task OnResourceLoaded(Host resource)
		{
			//  todo: API to store resource state
			//resource.State.ConnectedState = Host.ConnectedState.NotConnected;
			await StoreChanges(resource);

			var hostServer = new HostServer(resource,
				new EndpointConnectionManager(
					_eventBus, _endpointClientFactory, _serverIdentityVerifier,
					_loggerFactory.CreateLogger<EndpointConnectionManager>()
					),
				_eventBus,
				_loggerFactory.CreateLogger<HostServer>(),
				this
				);

			lock (_lock)
			{
				_hosts.Add(resource.HostId, hostServer);
			}

			_connectionManager.CreateConnection(hostServer);
			await base.OnResourceLoaded(resource);
		}
	}
}
