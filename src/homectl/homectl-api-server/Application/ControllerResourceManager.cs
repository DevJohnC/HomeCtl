using homectl_api_server.Resources;

namespace homectl_api_server.Application
{
	/// <summary>
	/// Specialization of KindManager for managing controllers.
	/// </summary>
	public class ControllerResourceManager : KindManager
	{
		public ControllerResourceManager(ResourceKind kind) : base(kind)
		{
		}
	}
}
