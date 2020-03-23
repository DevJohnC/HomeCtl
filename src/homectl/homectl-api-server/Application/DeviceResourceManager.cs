using homectl_api_server.Resources;

namespace homectl_api_server.Application
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
