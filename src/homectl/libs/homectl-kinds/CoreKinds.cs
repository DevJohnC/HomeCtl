using HomeCtl.Kinds.Resources;

namespace HomeCtl.Kinds
{
	public class CoreKinds
	{
		public const string KIND_GROUP_CORE = "core";
		public const string KIND_VERSION_V1ALPHA1 = "v1alpha1";

		public static readonly Kind<Kind> Kind = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "kind", "kinds",
			KindToDocument, DocumentToKind,
			metadata: metadata => { },
			spec: spec => { }
			);

		public static readonly Kind<Host> Host = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "host", "hosts",
			HostToDocument, DocumentToHost,
			metadata: metadata => { },
			state: state => { }
			);

		private static ResourceDocument? KindToDocument(Kind kind)
		{
			return null;
		}

		private static Kind? DocumentToKind(ResourceDocument resourceDocument)
		{
			return null;
		}

		private static ResourceDocument? HostToDocument(Host kind)
		{
			return null;
		}

		private static Host? DocumentToHost(ResourceDocument resourceDocument)
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
