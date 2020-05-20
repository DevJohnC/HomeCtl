using HomeCtl.Events;
using HomeCtl.Kinds;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Host
{
	public abstract class DeviceProviderBase : IDeviceProvider
	{
		public abstract IReadOnlyList<IResource> AvailableDevices { get; }
		protected EventBus EventBus { get; }

		public DeviceProviderBase(EventBus eventBus)
		{
			EventBus = eventBus;
		}

		public virtual Task MonitorForChanges(CancellationToken stoppingToken)
		{
			return Task.CompletedTask;
		}
	}
}
