using homectl.Application;

namespace homectl.Resources
{
	public class Host : Resource
	{
		public Host(ResourceMetadata metadata, ResourceSpec spec, ResourceState state) :
			base(ResourceManager.HostKind, metadata, spec, state)
		{
		}
	}
}
