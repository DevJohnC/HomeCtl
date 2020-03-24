using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace homectl_api_server.Resources
{
	public class Node : Resource
	{
		public static readonly ResourceSchema SCHEMA = new ResourceSchema(
			metadata: new OpenApiSchema(),
			state: new OpenApiSchema(),
			spec: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "hostname", new OpenApiSchema { Type = "string" } }
				},
				Required = new SortedSet<string>(new[] { "hostname" })
			});
	}
}
