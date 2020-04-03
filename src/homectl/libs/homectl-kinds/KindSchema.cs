using Microsoft.OpenApi.Models;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// Describes the structure of a kind.
	/// </summary>
	public class KindSchema
	{
		public KindSchema(OpenApiSchema metadataSchema, OpenApiSchema specSchema, OpenApiSchema? stateSchema)
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
		public OpenApiSchema SpecSchema { get; }

		/// <summary>
		/// Gets an OpenApi type schema that describes the structure of the kinds state, if it has one.
		/// </summary>
		public OpenApiSchema? StateSchema { get; }
	}
}
