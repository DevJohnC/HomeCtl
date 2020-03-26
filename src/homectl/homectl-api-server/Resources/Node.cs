using homectl.Application;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace homectl.Resources
{
	public class Node : Resource
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
					{ "hostname", new OpenApiSchema { Type = "string" } }
				},
				Required = new SortedSet<string>(new[] { "hostname" })
			});

		public Node(ResourceMetadata metadata, ResourceSpec spec, ResourceState state) :
			base(ResourceManager.NodeKind, metadata, spec, state)
		{
		}
	}
}
