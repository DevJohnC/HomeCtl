using HomeCtl.Kinds.Resources;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// Describes the structure of a kind.
	/// </summary>
	public class KindSchema
	{
		public KindSchema(OpenApiSchema metadataSchema,
			OpenApiSchema definitionSchema,
			OpenApiSchema? specSchema, OpenApiSchema? stateSchema)
		{
			MetadataSchema = metadataSchema;
			DefinitionSchema = definitionSchema;
			SpecSchema = specSchema;
			StateSchema = stateSchema;
		}

		/// <summary>
		/// Gets an OpenApi type schema that describes the structure of the kinds metadata.
		/// </summary>
		public OpenApiSchema MetadataSchema { get; }

		/// <summary>
		/// GEts an OpenApi type schema that describes the structure of the kinds definition.
		/// </summary>
		public OpenApiSchema DefinitionSchema { get; }

		/// <summary>
		/// Gets an OpenApi type schema that describes the structure of the kinds spec.
		/// </summary>
		public OpenApiSchema? SpecSchema { get; }

		/// <summary>
		/// Gets an OpenApi type schema that describes the structure of the kinds state, if it has one.
		/// </summary>
		public OpenApiSchema? StateSchema { get; }

		public static KindSchema FromKindDefinition(ResourceDefinition resourceDefinition)
		{
			var reader = new OpenApiStringReader();
			var metadataSchema = reader.ReadFragment<OpenApiSchema>(resourceDefinition["metadataSchema"]?.GetString() ?? throw new MissingResourceFieldException("metadataSchema"),
				Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0, out var _);

			var definitionSchema = reader.ReadFragment<OpenApiSchema>(resourceDefinition["definitionSchema"]?.GetString() ?? throw new MissingResourceFieldException("definitionSchema"),
				Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0, out var _);

			var specSchema = default(OpenApiSchema);
			var stateSchema = default(OpenApiSchema);

			if (resourceDefinition["specSchema"] != null)
				specSchema = reader.ReadFragment<OpenApiSchema>(resourceDefinition["specSchema"].GetString(),
					Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0, out var _);

			if (resourceDefinition["stateSchema"] != null)
				stateSchema = reader.ReadFragment<OpenApiSchema>(resourceDefinition["stateSchema"].GetString(),
					Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0, out var _);

			return new KindSchema(metadataSchema, definitionSchema, specSchema, stateSchema);
		}
	}
}
