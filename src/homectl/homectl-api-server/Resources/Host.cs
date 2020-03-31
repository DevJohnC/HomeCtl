using homectl.Application;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;

namespace homectl.Resources
{
	public class Host : Resource
	{
		public static readonly Guid KIND_RECORD_ID = Guid.Parse("D619E39F-7F6E-412C-9D56-5D6614E9027C");

		private static readonly ResourceSchema SCHEMA = new ResourceSchema(
			metadataSchema: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "hostname", new OpenApiSchema { Type = "string" } }
				},
				Required = { "hostname" }
			},
			specSchema: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "endpoint", new OpenApiSchema { Type = "string" } }
				},
				Required = { "endpoint" }
			},
			stateSchema: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "status", new OpenApiSchema { Type = "string", Default = new OpenApiString("disconnected"), Enum = new[] { new  OpenApiString("disconnected"), new OpenApiString("connecting"), new OpenApiString("connected") } } }
				}
			});

		public static readonly KindDescriptor DESCRIPTOR = new KindDescriptor(
			KindManager.KIND_GROUP_CORE,
			KindManager.KIND_VERSION_V1ALPHA1,
			"host",
			"hosts",
			SCHEMA
			);

		public Host(Kind kind, ResourceRecord record, ResourceMetadata metadata, ResourceSpec spec, ResourceState state) :
			base(kind, record, metadata, spec, state)
		{
		}
	}
}
