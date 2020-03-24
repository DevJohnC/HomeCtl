using Newtonsoft.Json.Linq;
using System;

namespace homectl_api_server.Resources
{
	public class ResourceMetadata : ResourceDocument<ResourceMetadata>
	{
		public static readonly ResourceMetadata Nothing = new ResourceMetadata();

		public Guid Id { get; protected set; }

		protected override void PopulateMembers(JToken jsonObject)
		{
			Id = Guid.Parse(jsonObject["id"].Value<string>());
		}
	}
}
