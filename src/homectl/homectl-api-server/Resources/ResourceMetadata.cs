using System;

namespace homectl_api_server.Resources
{
	public class ResourceMetadata : ResourceDocument<ResourceMetadata>
	{
		public static readonly ResourceMetadata Nothing = new ResourceMetadata();

		public Guid Id { get; }
	}
}
