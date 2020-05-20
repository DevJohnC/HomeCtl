using HomeCtl.Connection;
using HomeCtl.Events;
using HomeCtl.Kinds;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Host
{
	class HomeCtlHostService : BackgroundService
	{
		private readonly EventBus _eventBus;
		private readonly ApiServer _apiServer;
		private readonly EndpointConnectionManager _connectionManager;
		private readonly IEnumerable<IServerEndpointProvider> _serverEndpointProviders;
		private readonly IEnumerable<IServerLivelinessMonitor> _livelinessMonitors;
		private readonly IEnumerable<IDeviceProvider> _deviceProviders;

		public HomeCtlHostService(
			EventBus eventBus,
			ApiServer apiServer,
			EndpointConnectionManager connectionManager,
			IEnumerable<IServerEndpointProvider> serverEndpointProviders,
			IEnumerable<IServerLivelinessMonitor> livelinessMonitors,
			IEnumerable<IDeviceProvider> deviceProviders,
			IEnumerable<StartupService> startupServices //  ensure startup services are constructed before this service runs
			)
		{
			_eventBus = eventBus;
			_apiServer = apiServer;
			_connectionManager = connectionManager;
			_serverEndpointProviders = serverEndpointProviders;
			_livelinessMonitors = livelinessMonitors;
			_deviceProviders = deviceProviders;

			SubscribeToEvents();
		}

		private void SubscribeToEvents()
		{
			_eventBus.Subscribe<HostRecordsEvents.HostRecordApplied>(ConnectedToServer);
		}

		private async void ConnectedToServer(HostRecordsEvents.HostRecordApplied connected)
		{
			var devices = _deviceProviders.SelectMany(q => q.AvailableDevices).ToList();
			var kinds = devices.Select(q => q.Kind).GroupBy(q => q).Select(q => q.First()).ToList();

			foreach (var kind in kinds)
			{
				if (!CoreKinds.Kind.TryConvertToDocument(kind, out var document))
				{
					//  todo: log failure
					continue;
				}

				await _apiServer.Apply(document);
			}

			foreach (var device in devices)
			{
				if (!device.Kind.TryConvertToDocument(device, out var document))
				{
					//  todo: log failure
					continue;
				}

				await _apiServer.Apply(document);
			}
		}

		private Task RunDeviceProvider(IDeviceProvider deviceProvider, CancellationToken stoppingToken)
		{
			return deviceProvider.MonitorForChanges(stoppingToken);
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.WhenAll(
				_deviceProviders.Select(q => RunDeviceProvider(q, stoppingToken))
					.Concat(new[]
					{
						_connectionManager.Run(_serverEndpointProviders, _livelinessMonitors, stoppingToken)
					})
				);
		}
	}
}
