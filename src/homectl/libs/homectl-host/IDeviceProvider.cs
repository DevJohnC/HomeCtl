using HomeCtl.Kinds;
using System.Collections.Generic;

namespace HomeCtl.Host
{
	public interface IDeviceProvider
	{
		IReadOnlyList<IResource> AvailableDevices { get; }
	}
}
