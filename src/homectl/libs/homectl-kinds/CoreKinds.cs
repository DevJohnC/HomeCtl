using HomeCtl.Kinds.Resources;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.IO;

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
				.RequireString("name")
				.RequireString("namePlural")
				.RequireString("extendsKind"),
			definition: def => def.RequireString("identity"),
			spec: spec => spec
				.RequireString("metadataSchema", "json")
				.OptionalString("specSchema", "json")
				.OptionalString("stateSchema", "json")
			);

		public static readonly Kind<Host> Host = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "host", "hosts",
			HostToDocument, DocumentToHost,
			metadata: metadata => metadata
				.RequireString("machineName"),
			definition: def => def
				.RequireString("identity")
				.RequireString("endpoint"),
			state: state => state
				.RequireEnum<Host.ConnectedState>("connectedState")
			);

		public static readonly Kind<Controller> Controller = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "controller", "controllers",
			ControllerToDocument, DocumentToController,
			metadata: metadata => metadata
				.RequireString("hostId", format: "uuid"),
			definition: def => def.RequireString("identity"),
			state: state => state
				.OptionalObjectArray("intentFilters", filters => filters
					.OptionalString("action")
					.OptionalString("category"))
			);

		public static readonly Kind<Device> Device = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "device", "devices",
			DeviceToDocument, DocumentToDevice,
			metadata: metadata => { },
			definition: def => def.RequireString("identity")
			);

		private static string ReadStringField(ResourceFieldCollection? fields, string fieldName)
		{
			if (fields == null)
				throw new MissingResourceFieldException(fieldName);
			return fields[fieldName]?.GetString()
				?? throw new MissingResourceFieldException(fieldName);
		}

		private static string? WriteToJson(OpenApiSchema? schema)
		{
			if (schema == null)
				return null;

			using (var stringWriter = new StringWriter())
			{
				var writer = new OpenApiJsonWriter(stringWriter);
				schema.SerializeAsV3(writer);

				return stringWriter.ToString();
			}
		}

		private static ResourceDocument? KindToDocument(Kind kind)
		{
			return new ResourceDocument(
				new KindDescriptor(Kind.Group, Kind.ApiVersion, Kind.KindName),
				new ResourceMetadata(new List<ResourceField>
				{
					new ResourceField("group", ResourceFieldValue.String(kind.Group)),
					new ResourceField("apiVersion", ResourceFieldValue.String(kind.ApiVersion)),
					new ResourceField("name", ResourceFieldValue.String(kind.KindName)),
					new ResourceField("namePlural", ResourceFieldValue.String(kind.KindNamePlural)),
					new ResourceField("extendsKind", ResourceFieldValue.String(GetExtendsKindValue())),
				}),
				new ResourceDefinition(new List<ResourceField>
				{
					new ResourceField("identity", ResourceFieldValue.String($"{kind.Group}/{kind.ApiVersion}/{kind.KindName}")),
					new ResourceField("metadataSchema", ResourceFieldValue.String(WriteToJson(kind.Schema.MetadataSchema))),
					new ResourceField("definitionSchema", ResourceFieldValue.String(WriteToJson(kind.Schema.DefinitionSchema))),
					new ResourceField("specSchema", ResourceFieldValue.String(WriteToJson(kind.Schema.SpecSchema))),
					new ResourceField("stateSchema", ResourceFieldValue.String(WriteToJson(kind.Schema.StateSchema)))
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
			return DocumentToKind(resourceDocument, null);
		}

		public static Kind? DocumentToKind(ResourceDocument resourceDocument, Func<KindDescriptor, Kind?>? resolveExtensionKind)
		{
			if (resourceDocument.Kind.Group != Kind.Group ||
				resourceDocument.Kind.ApiVersion != Kind.ApiVersion ||
				resourceDocument.Kind.KindName != Kind.KindName)
				return null; //  not a kind

			return new SchemaDrivenKind(
				ReadStringField(resourceDocument.Metadata, "name"),
				ReadStringField(resourceDocument.Metadata, "namePlural"),
				ReadStringField(resourceDocument.Metadata, "group"),
				ReadStringField(resourceDocument.Metadata, "apiVersion"),
				KindSchema.FromKindDefinition(resourceDocument.Definition ?? throw new MissingResourceFieldException("definition")),
				ResolveExtensionKind(resourceDocument.Metadata["extendsKind"]?.GetString())
				);

			Kind? ResolveExtensionKind(string? kindDescriptorString)
			{
				if (string.IsNullOrWhiteSpace(kindDescriptorString))
					return default;

				var splitString = kindDescriptorString.Split('/');
				var kindDescriptor = new KindDescriptor(splitString[0], splitString[1], splitString[2]);
				return resolveExtensionKind?.Invoke(kindDescriptor);
			}
		}

		private static ResourceDocument? HostToDocument(Host host)
		{
			return new ResourceDocument(
				new KindDescriptor(Host.Group, Host.ApiVersion, Host.KindName),
				new ResourceMetadata(new List<ResourceField>
				{
					new ResourceField("machineName", ResourceFieldValue.String(host.MachineName))
				}),
				new ResourceDefinition(new List<ResourceField>
				{
					new ResourceField("identity", ResourceFieldValue.String(host.HostId.ToString())),
					new ResourceField("endpoint", ResourceFieldValue.String(host.Endpoint))
				}));
		}

		private static Host? DocumentToHost(ResourceDocument resourceDocument)
		{
			if (resourceDocument.Kind.Group != Host.Group ||
				resourceDocument.Kind.ApiVersion != Host.ApiVersion ||
				resourceDocument.Kind.KindName != Host.KindName)
				return null; //  not a host

			return new Host
			{
				HostId = Guid.Parse(ReadStringField(resourceDocument.Definition, "identity")),
				Endpoint = ReadStringField(resourceDocument.Definition, "endpoint"),
				MachineName = ReadStringField(resourceDocument.Metadata, "machineName")
			};
		}

		private static ResourceDocument? ControllerToDocument(Controller controller)
		{
			return null;
		}

		private static Controller? DocumentToController(ResourceDocument resourceDocument)
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
	}
}
