using homectl_api_server.Application;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace homectl_api_server.Resources
{
	/// <summary>
	/// A controller is responsible for applying state changes to resources.
	/// </summary>
	public class Controller : Resource
	{
		public static readonly ResourceSchema SCHEMA = new ResourceSchema(
			metadata: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "id", new OpenApiSchema { Type = "string", Format = "uuid" } }
				},
				Required = new SortedSet<string>(new[] { "id" })
			},
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

		public Controller(ResourceMetadata metadata, ResourceSpec spec, ResourceState state) :
			base(ResourceManager.ControllerKind, metadata, spec, state)
		{
		}
	}
}
