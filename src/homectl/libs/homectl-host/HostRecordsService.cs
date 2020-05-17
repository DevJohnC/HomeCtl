using HomeCtl.Connection;
using HomeCtl.Events;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HomeCtl.Host
{
	internal class HostRecordsService
	{
		private readonly int _hostPort;
		private readonly AppHost _appHost;
		private readonly ApiServer _apiServer;

		public HostRecordsService(EventBus eventBus, ApiServer apiServer, AppHost appHost,
			IOptions<HostOptions> hostOptions)
		{
			_hostPort = hostOptions.Value.HostPort;
			_appHost = appHost;
			_apiServer = apiServer;
			RegisterEventHandlers(eventBus);
		}

		private void RegisterEventHandlers(EventBus eventBus)
		{
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(Handle_NewConnection);
		}

		private async void Handle_NewConnection(EndpointConnectionEvents.Connected connectedArgs)
		{
			await ApplyHostConfigOnApiServer();
		}

		private async Task UpdateLocalEndpoint()
		{
			var ipAddress = await _apiServer.GetPerceivedIpAddress();
			if (IsIPV6(ipAddress))
				ipAddress = $"[{ipAddress}]";
			_appHost.State.Endpoint = $"http://{ipAddress}:{_hostPort}/";
		}

		private bool IsIPV6(string ipAddressStr)
		{
			if (!IPAddress.TryParse(ipAddressStr, out var ipAddress))
				return false;

			return ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
		}

		private async Task ApplyHostConfigOnApiServer()
		{
			try
			{
				await UpdateLocalEndpoint();
				await _apiServer.Apply(_appHost);
			}
			catch (Exception ex)
			{
				//  todo: keep attempting to notify the API server of the nodes presence?
				//  todo: log exception
			}
		}
	}
}
