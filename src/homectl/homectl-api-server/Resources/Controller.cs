using Microsoft.OpenApi.Models;

namespace homectl_api_server.Resources
{
	/// <summary>
	/// A controller is responsible for applying state changes to resources.
	/// </summary>
	public class Controller : Resource
	{
		public static readonly ResourceSchema SCHEMA = new ResourceSchema(
			metadata: new OpenApiSchema(),
			state: new OpenApiSchema(),
			spec: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "selector", new OpenApiSchema
						{
							Type = "object",
							Properties =
							{
								{ "matchNode", new OpenApiSchema { Type = "string" } },
								{ "matchDevice", new OpenApiSchema { Type = "string" } },
								{ "matchKind", new OpenApiSchema { Type = "string" } }
							}
						}
					}
				}
			});
	}
}
