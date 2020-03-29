using homectl;
using homectl.Devices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace homectl_host_IntegrationTests.Devices
{
	[TestClass]
	public class DeviceHostingTests
	{
		[TestMethod]
		public async Task Can_Create_Device()
		{
			var deviceProvider = new FakeDeviceProvider();

			using (var apiServer = new ApiServer())
			using (var httpClient = apiServer.CreateClient())
			using (var deviceHost = new DeviceHost(httpClient, deviceProvider))
			{
				deviceProvider.DeclareNewDevice();
			}
		}

		private class FakeDeviceProvider : IDeviceProvider
		{
			private readonly List<Device> _devices = new List<Device>();

			public IReadOnlyList<Device> AvailableDevices => _devices;

			public event EventHandler<DeviceAvailabilityChangeEventArgs> AvailableDevicesChanged;

			public void DeclareNewDevice()
			{
				var newDevice = new Device();
				_devices.Add(newDevice);
				AvailableDevicesChanged?.Invoke(this,
					new DeviceAvailabilityChangeEventArgs(newDevice, DeviceAvailabilityChangeEventArgs.OperationType.DeviceAdded));
			}
		}

		private class DeviceHost : IDisposable
		{
			private readonly HttpClient _httpClient;
			private readonly FakeDeviceProvider _deviceProvider;
			private readonly IHost _host;

			public DeviceHost(HttpClient httpClient, FakeDeviceProvider deviceProvider)
			{
				_httpClient = httpClient;
				_deviceProvider = deviceProvider;

				_host = CreateHost();
				_host.Start();
			}

			private IHost CreateHost()
				=> Host.CreateDefaultBuilder()
					.ConfigureCtlHostDefaults()
					.ConfigureServices(services =>
					{
						services.AddSingleton<ApiServerEndpoint>(new ConfiguredHttpClientEndpoint(_httpClient));
						services.AddSingleton<IDeviceProvider>(_deviceProvider);
					})
					.Build();

			public void Dispose()
			{
				_host.Dispose();
			}
		}
	}
}
