using homectl.Resources;

namespace homectl.Application
{
	/// <summary>
	/// Specialization of KindManager for managing controllers.
	/// </summary>
	public class ControllerResourceManager : ResourceManager
	{
		public ControllerResourceManager(Kind kind) : base(kind)
		{
		}
	}
}
