using Newtonsoft.Json.Linq;

namespace HomeCtl.ApiServer.Resources
{
	struct Resource
	{
		public Resource(ResourceRecord record, KindDescriptor kind, JObject metadataJson, JObject specJson, JObject? stateJson = null)
		{
			Record = record;
			MetadataJson = metadataJson;
			SpecJson = specJson;
			Kind = kind;
			StateJson = stateJson;
		}

		public ResourceRecord Record { get; }

		public JObject MetadataJson { get; }

		public JObject SpecJson { get; }

		public JObject? StateJson { get; }

		public KindDescriptor Kind { get; }
	}
}
