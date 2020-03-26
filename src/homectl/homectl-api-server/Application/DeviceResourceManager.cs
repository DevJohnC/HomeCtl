using homectl.Resources;

namespace homectl.Application
{
	/// <summary>
	/// Specialization of KindManager for managing devices.
	/// </summary>
	public class DeviceResourceManager : KindManager
	{
		public DeviceResourceManager(ResourceKind kind) : base(kind)
		{
		}
	}
}
