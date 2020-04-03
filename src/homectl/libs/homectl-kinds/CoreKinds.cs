using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace HomeCtl.Kinds
{
	public class CoreKinds
	{
		public const string KIND_GROUP_CORE = "core";
		public const string KIND_VERSION_V1ALPHA1 = "v1alpha1";

		public static readonly Kind Kind = new Kind("kind", "kinds", KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, new KindSchema(new OpenApiSchema(), new OpenApiSchema(), null), null);

		public static readonly Kind Host = new Kind("host", "hosts", KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, new KindSchema(
				metadataSchema: new OpenApiSchema
				{
					Type = "object",
					Properties =
					{
						{ "name", new OpenApiSchema { Type = "string" } },
						{ "hostname", new OpenApiSchema { Type = "string" } }
					},
					Required = { "hostname" }
				},
				specSchema: new OpenApiSchema(),
				stateSchema: new OpenApiSchema
				{
					Type = "object",
					Properties =
					{
						{ "endpoint", new OpenApiSchema { Type = "string" } },
						{ "status", new OpenApiSchema { Type = "string", Default = new OpenApiString("disconnected"), Enum = new[] { new  OpenApiString("disconnected"), new OpenApiString("connecting"), new OpenApiString("connected") } } }
					},
					Required = { "endpoint" }
				}
			), null);
	}
}
