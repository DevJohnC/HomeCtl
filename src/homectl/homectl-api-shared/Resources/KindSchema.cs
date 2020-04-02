using Microsoft.OpenApi.Models;

namespace homectl.Resources
{
	public class KindSchema
	{
		public static readonly KindSchema Empty = new KindSchema(
			new OpenApiSchema(),
			new OpenApiSchema(),
			null
			);

		public KindSchema(OpenApiSchema metadataSchema, OpenApiSchema specSchema, OpenApiSchema stateSchema)
		{
			MetadataSchema = metadataSchema;
			SpecSchema = specSchema;
			StateSchema = stateSchema;
		}

		public OpenApiSchema MetadataSchema { get; }

		public OpenApiSchema SpecSchema { get; }

		public bool HasStateSchema => StateSchema != null;

		public OpenApiSchema StateSchema { get; }
	}
}
