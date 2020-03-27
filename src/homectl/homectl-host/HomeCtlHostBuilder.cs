using homectl.Devices;
using System.Collections.Generic;

namespace homectl
{
	public class HomeCtlHostBuilder
	{
		private IStartupClassInvoker _startupClassInvoker;
		private readonly List<IDeviceProvider> _deviceProviders = new List<IDeviceProvider>();

		public HomeCtlHostBuilder UseStartup<TStartup>()
			where TStartup : HomeCtlStartup, new()
		{
			_startupClassInvoker = new StartupClassInvoker<TStartup>();
			return this;
		}

		public HomeCtlHostBuilder AddDevices(IDeviceProvider deviceProvider)
		{
			_deviceProviders.Add(deviceProvider);
			return this;
		}

		public HomeCtlHost Build()
		{
			_startupClassInvoker?.Configure(this);
			return new HomeCtlHost(
				new ConnectionManager(),
				default,
				_deviceProviders
				);
		}

		private interface IStartupClassInvoker
		{
			void Configure(HomeCtlHostBuilder builder);
		}

		private class StartupClassInvoker<TStartup> : IStartupClassInvoker
			where TStartup : HomeCtlStartup, new()
		{
			public void Configure(HomeCtlHostBuilder builder)
			{
				new TStartup().Configure(builder);
			}
		}
	}
}
