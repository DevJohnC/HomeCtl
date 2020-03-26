using homectl.Resources;

namespace homectl.Application
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
