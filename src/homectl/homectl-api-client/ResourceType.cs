using homectl.Resources;

namespace homectl
{
	public abstract class ResourceType
	{
		public static readonly ResourceType<HostResource> Host = new ResourceType<HostResource>();
	}

	public class ResourceType<TResource> : ResourceType
	{
	}
}
