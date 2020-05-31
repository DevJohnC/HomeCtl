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
			HostManager hostManager,
			ResourceStateManager resourceStateManager) :
			base(connectionManager, eventBus, logger)
		{
			Host = host;
			_hostManager = hostManager;
			_resourceStateManager = resourceStateManager;
		}

		public Host Host { get; }

		private readonly HostManager _hostManager;
		private readonly ResourceStateManager _resourceStateManager;

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
			if (args.ServerEndpoint.Uri.OriginalString != Host.Endpoint)
				return;

			await Host.SetConnectedState(_resourceStateManager, Host.ConnectedState.Connected);
		}

		private async void Handle_Disconnected(EndpointConnectionEvents.Disconnected args)
		{
			if (args.ServerEndpoint.Uri.OriginalString != Host.Endpoint)
				return;

			await Host.SetConnectedState(_resourceStateManager, Host.ConnectedState.NotConnected);
		}
	}
}
