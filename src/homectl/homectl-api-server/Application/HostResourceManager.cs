using homectl.Resources;
using System;

namespace homectl.Application
{
	public class HostResourceManager : ResourceManager
	{
		public HostResourceManager(Kind kind) : base(kind)
		{
		}

		public override Resource? Create(ResourceMetadata metadata, ResourceSpec spec, Guid? resourceIdentifier)
		{
			var host = new Host(Kind, new ResourceRecord(resourceIdentifier ?? Guid.NewGuid()),
				metadata, spec, ResourceState.Nothing);
			Add(host);
			return host;
		}
	}
}
