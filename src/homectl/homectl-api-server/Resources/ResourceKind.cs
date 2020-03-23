using System;

namespace homectl_api_server.Resources
{
	public class ResourceKind : Resource
	{
		public ResourceKind(string group, string apiVersion, string kindName)
		{
			Group = group ?? throw new ArgumentNullException(nameof(group));
			ApiVersion = apiVersion ?? throw new ArgumentNullException(nameof(apiVersion));
			KindName = kindName ?? throw new ArgumentNullException(nameof(kindName));
		}

		public string Group { get; }

		public string ApiVersion { get; }

		public string KindName { get; }

		public Resource Create(ResourceMetadata metadata, ResourceSpec desiredSpec)
		{
			if (!Spec.Validate(desiredSpec))
				return default;

			return default;
		}
	}
}
