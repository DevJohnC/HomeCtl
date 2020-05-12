using HomeCtl.Kinds.Resources;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.Xml;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// Describes the structure of a kind.
	/// </summary>
	public class KindSchema
	{
		public KindSchema(OpenApiSchema metadataSchema, OpenApiSchema? specSchema, OpenApiSchema? stateSchema)
		{
			MetadataSchema = metadataSchema;
			SpecSchema = specSchema;
			StateSchema = stateSchema;
		}

		/// <summary>
		/// Gets an OpenApi type schema that describes the structure of the kinds metadata.
		/// </summary>
		public OpenApiSchema MetadataSchema { get; }

		/// <summary>
		/// Gets an OpenApi type schema that describes the structure of the kinds spec.
		/// </summary>
		public OpenApiSchema? SpecSchema { get; }

		/// <summary>
		/// Gets an OpenApi type schema that describes the structure of the kinds state, if it has one.
		/// </summary>
		public OpenApiSchema? StateSchema { get; }

		public static KindSchema FromKindSpec(ResourceSpec resourceSpec)
		{
			var reader = new OpenApiStringReader();
			var metadataSchema = reader.ReadFragment<OpenApiSchema>(resourceSpec["metadataSchema"]?.GetString() ?? throw new MissingResourceFieldException("metadataSchema"),
				Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0, out var _);

			var specSchema = default(OpenApiSchema);
			var stateSchema = default(OpenApiSchema);

			if (resourceSpec["specSchema"] != null)
				specSchema = reader.ReadFragment<OpenApiSchema>(resourceSpec["specSchema"].GetString(),
					Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0, out var _);

			if (resourceSpec["stateSchema"] != null)
				stateSchema = reader.ReadFragment<OpenApiSchema>(resourceSpec["stateSchema"].GetString(),
					Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0, out var _);

			return new KindSchema(metadataSchema, specSchema, stateSchema);
		}
	}
}
