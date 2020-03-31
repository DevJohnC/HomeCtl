using homectl.Resources;

namespace homectl.Application
{
	/// <summary>
	/// Specialization of KindManager for managing devices.
	/// </summary>
	public class DeviceResourceManager : ResourceManager
	{
		public DeviceResourceManager(Kind kind) : base(kind)
		{
		}
	}
}
