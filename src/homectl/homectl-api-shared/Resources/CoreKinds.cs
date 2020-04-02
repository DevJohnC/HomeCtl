using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;

namespace homectl.Resources
{
	public class CoreKinds
	{
		public const string KIND_GROUP_CORE = "core";
		public const string KIND_VERSION_V1ALPHA1 = "v1alpha1";

		private static readonly Guid KIND_ID_KIND = Guid.Parse("C2708787-0FF0-49A8-9CC8-EF79344653FC");
		private static readonly Guid KIND_ID_HOST = Guid.Parse("44D8A396-EF09-4387-9D36-F6461DBFD67A");

		public readonly static TypeDescriptor<KindResource> Kind = new TypeDescriptor<KindResource>(
			KIND_ID_KIND, KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "kind", "kinds", KindSchema.Empty,
			kind => kind.Metadata, kind => kind.Spec
			);

		public readonly static TypeDescriptor<HostResource> Host = new TypeDescriptor<HostResource>(
			KIND_ID_HOST, KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "host", "hosts", new KindSchema(
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
				}),
				host => host.Metadata, host => host.Spec, host => host.State);
	}
}
