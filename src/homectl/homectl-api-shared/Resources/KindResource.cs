using Microsoft.OpenApi.Models;

namespace homectl.Resources
{
	public class KindResource : IResource<KindResource.KindMetadata, KindResource.KindSpec>
	{
		public KindResource(KindDescriptor kindDescriptor)
		{
			Metadata = new KindMetadata
			{
				Group = kindDescriptor.Group,
				ApiVersion = kindDescriptor.ApiVersion,
				KindName = kindDescriptor.KindName,
				KindNamePlural = kindDescriptor.KindNamePlural
			};
			if (kindDescriptor.ExtendsKind != null)
				Metadata.ExtendsKind = $"{kindDescriptor.Group}/{kindDescriptor.ApiVersion}/{kindDescriptor.KindName}";

			Spec = new KindSpec
			{
				MetadataSchema = kindDescriptor.Schema.MetadataSchema,
				SpecSchema = kindDescriptor.Schema.SpecSchema
			};
			if (kindDescriptor.Schema.HasStateSchema)
				Spec.StateSchema = kindDescriptor.Schema.StateSchema;
		}

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
			public OpenApiSchema MetadataSchema { get; set; }

			public OpenApiSchema SpecSchema { get; set; }

			public OpenApiSchema StateSchema { get; set; }
		}
	}
}
