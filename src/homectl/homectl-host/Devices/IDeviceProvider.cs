using System;

namespace homectl.Devices
{
	public interface IDeviceProvider
	{
		event EventHandler<DeviceAvailabilityChangeEventArgs> AvailableDevicesChanged;

		Device AvailableDevices { get; }
	}

	public class DeviceAvailabilityChangeEventArgs : EventArgs
	{
		public DeviceAvailabilityChangeEventArgs(Device device, OperationType operation)
		{
			Device = device;
			Operation = operation;
		}

		public Device Device { get; }

		public OperationType Operation { get; }

		public enum OperationType
		{
			Added,
			Removed
		}
	}
}
