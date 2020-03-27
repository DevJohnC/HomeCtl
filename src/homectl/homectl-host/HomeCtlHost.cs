using homectl.Controllers;
using homectl.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace homectl
{
	public class HomeCtlHost : IDisposable
	{
		public static HomeCtlHostBuilder CreateBuilder()
		{
			return new HomeCtlHostBuilder();
		}

		public HomeCtlHost(
			ConnectionManager connectionManager,
			IEnumerable<Controller> controllers,
			IEnumerable<IDeviceProvider> deviceProviders
			)
		{
			ConnectionManager = connectionManager;
			Controllers = controllers?.ToArray() ?? new Controller[0];
			DeviceProviders = deviceProviders?.ToArray() ?? new IDeviceProvider[0];

			AttachEventHandlers(ConnectionManager);
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

		public ConnectionManager ConnectionManager { get; }
		public Controller[] Controllers { get; }
		public IDeviceProvider[] DeviceProviders { get; }

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		private void DisposeManagedResources()
		{
			DetachEventHandlers(ConnectionManager);
			ConnectionManager.Dispose();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					DisposeManagedResources();
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
