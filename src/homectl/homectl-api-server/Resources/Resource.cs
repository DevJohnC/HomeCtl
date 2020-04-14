using HomeCtl.Kinds;
using Newtonsoft.Json.Linq;

namespace HomeCtl.ApiServer.Resources
{
	struct Resource
	{
		public Resource(JObject metadataJson, JObject specJson, Kind kind) : this()
		{
			MetadataJson = metadataJson;
			SpecJson = specJson;
			Kind = kind;
		}

		public Resource(JObject metadataJson, JObject specJson, JObject? stateJson, Kind kind)
		{
			MetadataJson = metadataJson;
			SpecJson = specJson;
			StateJson = stateJson;
			Kind = kind;
		}

		public JObject MetadataJson { get; set; }

		public JObject SpecJson { get; set; }

		public JObject? StateJson { get; set; }

		public Kind Kind { get; }
	}
}
