using HomeCtl.ApiServer.Resources;
using HomeCtl.Connection;
using HomeCtl.Events;
using HomeCtl.Kinds;
using Microsoft.Extensions.Logging;

namespace HomeCtl.ApiServer.Hosts
{
	class HostServer : Server
	{
		public HostServer(Host host, 
			EndpointConnectionManager connectionManager, EventBus eventBus,
			ILogger<HostServer> logger,
			HostManager hostManager) :
			base(connectionManager, eventBus, logger)
		{
			Host = host;
			_hostManager = hostManager;
		}

		public Host Host { get; }

		private readonly HostManager _hostManager;

		protected override void SubscribeToEvents()
		{
			EventBus.Subscribe<EndpointConnectionEvents.Connected>(Handle_Connected);
			EventBus.Subscribe<EndpointConnectionEvents.Disconnected>(Handle_Disconnected);
		}

		protected override void UnsubscribeFromEvents()
		{
			EventBus.Unsubscribe<EndpointConnectionEvents.Connected>(Handle_Connected);
			EventBus.Unsubscribe<EndpointConnectionEvents.Disconnected>(Handle_Disconnected);
		}

		private async void Handle_Connected(EndpointConnectionEvents.Connected args)
		{
			if (args.ServerEndpoint.Uri.OriginalString != Host.State.Endpoint)
				return;

			Host.State.ConnectedState = Host.ConnectedState.Connected;

			await _hostManager.StoreChanges(Host);
		}

		private async void Handle_Disconnected(EndpointConnectionEvents.Disconnected args)
		{
			if (args.ServerEndpoint.Uri.OriginalString != Host.State.Endpoint)
				return;

			Host.State.ConnectedState = Host.ConnectedState.NotConnected;

			await _hostManager.StoreChanges(Host);
		}
	}
}
