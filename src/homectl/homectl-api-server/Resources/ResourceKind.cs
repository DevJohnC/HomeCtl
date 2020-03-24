namespace homectl_api_server.Resources
{
	public class ResourceKind : Resource
	{
		public new static readonly ResourceKind Nothing = new ResourceKind(string.Empty, string.Empty, string.Empty, ResourceSchema.Nothing);

		public ResourceKind(string group, string apiVersion, string kindName,
			ResourceSchema schema)
		{
			Group = group;
			ApiVersion = apiVersion;
			KindName = kindName;
			Schema = schema;
		}

		public string Group { get; }

		public string ApiVersion { get; }

		public string KindName { get; }

		public ResourceSchema Schema { get; }

		public Resource Create(ResourceMetadata metadata, ResourceSpec desiredSpec)
		{
			//if (!Spec.Validate(desiredSpec))
			//	return Resource.Nothing;

			return Resource.Nothing;
		}
	}
}
