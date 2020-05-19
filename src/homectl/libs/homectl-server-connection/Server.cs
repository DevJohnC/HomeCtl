using HomeCtl.Events;
using HomeCtl.Services;
using HomeCtl.Services.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public abstract class Server : IDisposable
	{
		protected Server(EndpointConnectionManager connectionManager,
			EventBus eventBus, ILogger logger)
		{
			ConnectionManager = connectionManager;
			EventBus = eventBus;
			Logger = logger;

			SubscribeToEventsImpl();
		}

		public ConnectionStatus ConnectionStatus => ConnectionManager.ConnectionStatus;

		public EndpointConnectionManager ConnectionManager { get; }

		public ServerVersion ServerVersion { get; private set; }

		protected EventBus EventBus { get; }

		protected ILogger Logger { get; }

		public NetworkTiming NetworkTiming { get; private set; }

		protected virtual void SubscribeToEvents()
		{
			EventBus.Subscribe<EndpointConnectionEvents.Connected>(Handle_NewConnection);
		}

		protected virtual void UnsubscribeFromEvents()
		{
			EventBus.Unsubscribe<EndpointConnectionEvents.Connected>(Handle_NewConnection);
		}

		private void SubscribeToEventsImpl()
		{
			SubscribeToEvents();
		}

		private void UnsubscribeFromEventsImpl()
		{
			UnsubscribeFromEvents();
		}

		public void Dispose()
		{
			UnsubscribeFromEventsImpl();
		}

		private async void Handle_NewConnection(EndpointConnectionEvents.Connected connectedArgs)
		{
			try
			{
				ServerVersion = await GetServerVersion();

				Logger.LogDebug($"Connected to server {ServerVersion} @ {connectedArgs.ServerEndpoint.Uri}");
			}
			catch
			{
				Logger.LogDebug($"Failed to determine the version of a server @ {connectedArgs.ServerEndpoint.Uri}");
			}
		}

		/// <summary>
		/// Gets the local nodes IP address as seen by the API server.
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetPerceivedIpAddress()
		{
			var client = new Information.InformationClient(ConnectionManager.ServicesChannel);
			var response = await client.GetClientIpAddressAsync(Empty.Instance);
			return response.IpAddress;
		}

		public async Task<ServerVersion> GetServerVersion()
		{
			var client = new Information.InformationClient(ConnectionManager.ServicesChannel);
			var version = await client.GetServerVersionAsync(Empty.Instance);
			return new ServerVersion(version.ApiServerVersion.Major, version.ApiServerVersion.Minor,
				version.ApiServerVersion.Name);
		}

		public async Task<NetworkTiming> GetNetworkTiming()
		{
			var client = new Network.NetworkClient(ConnectionManager.ServicesChannel);
			var startTime = DateTimeOffset.UtcNow;
			var timingResponse = await client.GetNetworkTimingAsync(Empty.Instance);
			var receivedAtTime = DateTimeOffset.FromUnixTimeMilliseconds(timingResponse.ReceivedAtUnixTime);
			var endTime = DateTimeOffset.UtcNow;
			var roundTripTime = endTime - startTime;
			var ret = new NetworkTiming(roundTripTime, receivedAtTime - startTime);
			NetworkTiming = ret;
			return ret;
		}
	}
}
