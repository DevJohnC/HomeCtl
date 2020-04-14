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
						{ "id", new OpenApiSchema { Type = "string" } },
						{ "label", new OpenApiSchema { Type = "string" } },
						{ "hostname", new OpenApiSchema { Type = "string" } }
					},
					Required = { "hostname" }
				},
				specSchema: new OpenApiSchema
				{
					Type = "object",
					Properties =
					{
						{ "ipAddress", new OpenApiSchema { Type = "string", Format = "ipvAny", Description = "IP address the host can be contact on" } },
						{ "port", new OpenApiSchema { Type = "integer", Format = "int32", Minimum = 1024, Maximum = 65535, Description = "Port number the host can be contacted on" } }
					}
				},
				stateSchema: new OpenApiSchema
				{
					Type = "object",
					Properties =
					{
						{ "endpoint", new OpenApiSchema { Type = "string" } }
					}
				}
			), null);
	}
}
