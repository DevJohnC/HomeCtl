using HomeCtl.Kinds.Resources;
using System;
using System.Collections.Generic;

namespace HomeCtl.Kinds
{
	public class CoreKinds
	{
		public const string KIND_GROUP_CORE = "core";
		public const string KIND_VERSION_V1ALPHA1 = "v1alpha1";

		public static readonly Kind<Kind> Kind = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "kind", "kinds",
			KindToDocument, DocumentToKind,
			metadata: metadata => metadata
				.RequireString("group")
				.RequireString("apiVersion")
				.RequireString("kindName")
				.RequireString("kindNamePlural")
				.RequireString("extendsKind"),
			spec: spec => spec
				.RequireString("metadataSchema", "json")
				.OptionalString("specSchema", "json")
				.OptionalString("stateSchema", "json")
			);

		public static readonly Kind<Host> Host = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "host", "hosts",
			HostToDocument, DocumentToHost,
			metadata: metadata => { },
			state: state => { }
			);

		public static readonly Kind<Device> Device = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "device", "devices",
			DeviceToDocument, DocumentToDevice,
			metadata: metadata => { }
			);

		private static ResourceDocument? KindToDocument(Kind kind)
		{
			return new ResourceDocument(
				new KindDescriptor(Kind.Group, Kind.ApiVersion, Kind.KindName),
				new ResourceMetadata(Guid.Empty, "", new List<ResourceField>
				{
					new ResourceField("group", ResourceFieldValue.String(kind.Group)),
					new ResourceField("apiVersion", ResourceFieldValue.String(kind.ApiVersion)),
					new ResourceField("kindName", ResourceFieldValue.String(kind.KindName)),
					new ResourceField("kindNamePlural", ResourceFieldValue.String(kind.KindNamePlural)),
					new ResourceField("extendsKind", ResourceFieldValue.String(GetExtendsKindValue())),
				}),
				spec: new ResourceSpec(new List<ResourceField>
				{
					new ResourceField("metadataSchema", ResourceFieldValue.String("")),
					new ResourceField("specSchema", ResourceFieldValue.String("")),
					new ResourceField("stateSchema", ResourceFieldValue.String(""))
				}));

			string GetExtendsKindValue()
			{
				if (kind.ExtendsKind != null)
					return $"{kind.ExtendsKind.Group}/{kind.ExtendsKind.ApiVersion}/{kind.ExtendsKind.KindName}";
				return "";
			}
		}

		private static Kind? DocumentToKind(ResourceDocument resourceDocument)
		{
			return null;
		}

		private static ResourceDocument? HostToDocument(Host host)
		{
			return null;
		}

		private static Host? DocumentToHost(ResourceDocument resourceDocument)
		{
			return null;
		}

		private static ResourceDocument? DeviceToDocument(Device device)
		{
			return null;
		}

		private static Device? DocumentToDevice(ResourceDocument resourceDocument)
		{
			return null;
		}

		/*public static readonly Kind Kind = new Kind("kind", "kinds", KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, new KindSchema(
			metadataSchema: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "id", new OpenApiSchema { Type = "string", Format = "uuid" } },
					{ "label", new OpenApiSchema { Type = "string" } },
					{ "group", new OpenApiSchema { Type = "string" } },
					{ "apiVersion", new OpenApiSchema { Type = "string" } },
					{ "kindName", new OpenApiSchema { Type = "string" } }
				}
			},
			new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "metadataSchema", new OpenApiSchema { Type = "object", AdditionalPropertiesAllowed = true } },
					{ "specSchema", new OpenApiSchema { Type = "object", AdditionalPropertiesAllowed = true } },
					{ "stateSchema", new OpenApiSchema { Type = "object", AdditionalPropertiesAllowed = true } }
				}
			},
			null), null);

		public static readonly Kind Host = new Kind("host", "hosts", KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, new KindSchema(
				metadataSchema: new OpenApiSchema
				{
					Type = "object",
					Properties =
					{
						{ "id", new OpenApiSchema { Type = "string", Format = "uuid" } },
						{ "label", new OpenApiSchema { Type = "string" } },
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
						{ "endpoint", new OpenApiSchema { Type = "string" } }
					}
				}
			), null);*/
	}
}
