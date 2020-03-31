using Microsoft.OpenApi.Models;

namespace homectl.Resources
{
	public class KindDescriptor
	{
		public KindDescriptor(string group, string apiVersion, string kindName, string kindNamePlural, ResourceSchema schema)
		{
			Group = group;
			ApiVersion = apiVersion;
			KindName = kindName;
			KindNamePlural = kindNamePlural;
			Schema = schema;
		}

		public string Group { get; }

		public string ApiVersion { get; }

		public string KindName { get; }

		public string KindNamePlural { get; }

		public ResourceSchema Schema { get; }
	}

	public class ResourceSchema
	{
		public readonly static ResourceSchema Nothing = new ResourceSchema(new OpenApiSchema(), new OpenApiSchema());

		public ResourceSchema(OpenApiSchema metadataSchema, OpenApiSchema specSchema)
		{
			MetadataSchema = metadataSchema;
			SpecSchema = specSchema;
		}

		public ResourceSchema(OpenApiSchema metadataSchema, OpenApiSchema specSchema, OpenApiSchema stateSchema) :
			this (metadataSchema, specSchema)
		{
			StateSchema = stateSchema;
		}

		public OpenApiSchema MetadataSchema { get; }

		public OpenApiSchema SpecSchema { get; }

		public bool HasStateSchema => StateSchema != null;

		public OpenApiSchema? StateSchema { get; }
	}
}
