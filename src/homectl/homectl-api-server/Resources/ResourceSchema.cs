using Microsoft.OpenApi.Models;

namespace homectl.Resources
{
	public class ResourceSchema
	{
		public readonly static ResourceSchema Nothing = new ResourceSchema(new OpenApiSchema(), new OpenApiSchema(), new OpenApiSchema());

		public ResourceSchema(OpenApiSchema metadata, OpenApiSchema spec, OpenApiSchema state)
		{
			Metadata = metadata;
			Spec = spec;
			State = state;
		}

		public OpenApiSchema Metadata { get; }

		public OpenApiSchema Spec { get; }

		public OpenApiSchema State { get; }
	}
}
