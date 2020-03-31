using homectl.Application;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using System;

namespace homectl.Resources
{
	/// <summary>
	/// A kind instance that describes a homectl kind.
	/// </summary>
	public class Kind : Resource
	{
		public static readonly Guid KIND_RECORD_ID = Guid.Parse("B9661786-088D-4B98-939E-A22BB2011EF4");

		private static readonly ResourceSchema SCHEMA = new ResourceSchema(
			metadataSchema: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "group", new OpenApiSchema { Type = "string" } },
					{ "apiVersion", new OpenApiSchema { Type = "string" } },
					{ "kindName", new OpenApiSchema { Type = "string" } },
					{ "kindNamePlural", new OpenApiSchema { Type = "string" } },
					{ "extends", new OpenApiSchema { Type = "string" } }
				},
				Required = { "group", "apiVersion", "name", "plural" }
			},
			specSchema: new OpenApiSchema
			{
				Type = "object",
				Properties =
				{
					{ "metadataSchema", new OpenApiSchema { Type = "any" } },
					{ "specSchema", new OpenApiSchema { Type = "any" } },
					{ "stateSchema", new OpenApiSchema { Type = "any" } }
				},
				Required = { "endpoint" }
			});

		public static readonly KindDescriptor DESCRIPTOR = new KindDescriptor(
			KindManager.KIND_GROUP_CORE,
			KindManager.KIND_VERSION_V1ALPHA1,
			"kind",
			"kinds",
			SCHEMA
			);

		internal static Kind CreateKindKind()
		{
			var kindResource = new Kind();
			kindResource.Kind = kindResource;
			return kindResource;
		}

		public Kind(Kind kind, ResourceRecord record, KindDescriptor descriptor, ResourceState state) :
			base(kind, record, KindMetadata.FromDescriptor(descriptor), KindSpec.FromDescriptor(descriptor), state)
		{
			Group = descriptor.Group;
			ApiVersion = descriptor.ApiVersion;
			KindName = descriptor.KindName;
			Schema = descriptor.Schema;
		}

		private Kind() :
			base(new ResourceRecord(KIND_RECORD_ID), KindMetadata.FromDescriptor(DESCRIPTOR), KindSpec.FromDescriptor(DESCRIPTOR), ResourceState.Nothing)
		{
			Group = DESCRIPTOR.Group;
			ApiVersion = DESCRIPTOR.ApiVersion;
			KindName = DESCRIPTOR.KindName;
			Schema = DESCRIPTOR.Schema;
		}

		public string Group { get; }

		public string ApiVersion { get; }

		public string KindName { get; }

		public ResourceSchema Schema { get; }

		public class KindMetadata : ResourceMetadata
		{
			public string Group
			{
				get => Json.Value<string>(nameof(Group));
				set => Json[nameof(Group)] = value;
			}

			public string ApiVersion
			{
				get => Json.Value<string>(nameof(ApiVersion));
				set => Json[nameof(ApiVersion)] = value;
			}

			public string KindName
			{
				get => Json.Value<string>(nameof(KindName));
				set => Json[nameof(KindName)] = value;
			}

			public string KindNamePlural
			{
				get => Json.Value<string>(nameof(KindNamePlural));
				set => Json[nameof(KindNamePlural)] = value;
			}

			public string Extends
			{
				get => Json.Value<string>(nameof(Extends));
				set => Json[nameof(Extends)] = value;
			}

			public static KindMetadata FromDescriptor(KindDescriptor descriptor)
			{
				return new KindMetadata
				{
					Group = descriptor.Group,
					ApiVersion = descriptor.ApiVersion,
					KindName = descriptor.KindName,
					KindNamePlural = descriptor.KindNamePlural
				};
			}
		}

		public class KindSpec : ResourceSpec
		{
			public JToken MetadataSchema
			{
				get => Json.Value<JToken>(nameof(MetadataSchema));
				set => Json[nameof(MetadataSchema)] = value;
			}

			public JToken SpecSchema
			{
				get => Json.Value<JToken>(nameof(SpecSchema));
				set => Json[nameof(SpecSchema)] = value;
			}

			public JToken? StateSchema
			{
				get => Json.Value<JToken>(nameof(StateSchema));
				set => Json[nameof(StateSchema)] = value;
			}

			public static KindSpec FromDescriptor(KindDescriptor descriptor)
			{
				var result = new KindSpec
				{
					MetadataSchema = ConvertToJson(descriptor.Schema.MetadataSchema),
					SpecSchema = ConvertToJson(descriptor.Schema.SpecSchema)
				};

				if (descriptor.Schema.HasStateSchema)
					result.StateSchema = ConvertToJson(descriptor.Schema.StateSchema);

				return result;
			}

			private static JToken ConvertToJson(OpenApiSchema schema)
			{
				using (var textWriter = new System.IO.StringWriter())
				{
					var writer = new OpenApiJsonWriter(textWriter);
					schema.SerializeAsV3WithoutReference(writer);
					return JToken.Parse(textWriter.ToString());
				}
			}
		}
	}
}
