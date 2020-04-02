using homectl.Resources;
using Newtonsoft.Json.Linq;
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

		public override bool TryCreate(JObject metadata, JObject spec, Guid? resourceIdentifier, out ResourceRecordPair resourceRecordPair)
		{
			var record = new ResourceRecord(resourceIdentifier ?? Guid.NewGuid());
			var host = new HostResource
			{
				Metadata = HostResource.HostMetadata.FromJson(metadata),
				Spec = HostResource.HostSpec.FromJson(spec),
				State = new HostResource.HostState()
			};
			Add(record, host);
			resourceRecordPair = new ResourceRecordPair(record, host);
			return true;
		}
	}
}
