using homectl.Resources;
using System;

namespace homectl.Application
{
	public class HostManager : ResourceManager
	{
		public HostManager() : base(CoreKinds.Host)
		{
		}

		public ResourceRecordPair Create(object metadata, object spec, Guid? resourceIdentifier)
		{
			return default;
			//var host = new HostResource(Kind, new ResourceRecord(resourceIdentifier ?? Guid.NewGuid()),
			//	metadata, spec, ResourceState.Nothing);
			//Add(host);
			//return host;
		}
	}
}
