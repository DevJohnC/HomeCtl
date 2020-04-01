namespace homectl.Resources
{
	public class KindResource : IResource<KindResource.KindMetadata, KindResource.KindSpec>
	{
		public KindMetadata Metadata { get; set; }

		public KindSpec Spec { get; set; }

		public class KindMetadata
		{
			public string Group { get; set; }

			public string ApiVersion { get; set; }

			public string KindName { get; set; }

			public string KindNamePlural { get; set; }

			public string ExtendsKind { get; set; }
		}

		public class KindSpec
		{
			public OpenApiTypeSchema MetadataSchema { get; set; }

			public OpenApiTypeSchema SpecSchema { get; set; }

			public OpenApiTypeSchema StateSchema { get; set; }
		}
	}
}
