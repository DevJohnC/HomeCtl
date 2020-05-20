using HomeCtl.Kinds;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Host
{
	public interface IDeviceProvider
	{
		IReadOnlyList<IResource> AvailableDevices { get; }

		Task MonitorForChanges(CancellationToken stoppingToken);
	}
}
