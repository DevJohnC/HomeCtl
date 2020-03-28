using homectl.Controllers;
using homectl.Devices;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace homectl
{
	public class HomeCtlHost : IHostedService
	{
		public HomeCtlHost(
			ConnectionManager connectionManager,
			IEnumerable<Controller> controllers,
			IEnumerable<IDeviceProvider> deviceProviders
			)
		{
			ConnectionManager = connectionManager;
			Controllers = controllers?.ToArray() ?? new Controller[0];
			DeviceProviders = deviceProviders?.ToArray() ?? new IDeviceProvider[0];
		}

		public ConnectionManager ConnectionManager { get; }
		public Controller[] Controllers { get; }
		public IDeviceProvider[] DeviceProviders { get; }

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			AttachEventHandlers(ConnectionManager);
		}

		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			DetachEventHandlers(ConnectionManager);
		}

		private void AttachEventHandlers(ConnectionManager connectionManager)
		{
			connectionManager.Connected += ConnectionManager_Connected;
		}

		private void DetachEventHandlers(ConnectionManager connectionManager)
		{
			connectionManager.Connected -= ConnectionManager_Connected;
		}

		private void ConnectionManager_Connected(object sender, ConnectionEventArgs e)
		{
			//  connect to grpc event stream
		}
	}
}
