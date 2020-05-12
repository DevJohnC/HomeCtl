using HomeCtl.Kinds.Resources;
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
				.RequireString("name")
				.RequireString("namePlural")
				.RequireString("extendsKind"),
			spec: spec => spec
				.RequireString("metadataSchema", "json")
				.OptionalString("specSchema", "json")
				.OptionalString("stateSchema", "json")
			);

		public static readonly Kind<Host> Host = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "host", "hosts",
			HostToDocument, DocumentToHost,
			metadata: metadata => metadata
				.RequireString("hostId", format: "uuid")
				.RequireString("machineName"),
			state: state => state
				.RequireString("endpoint")
				.RequireEnum<Host.ConnectedState>("connectedState")
			);

		public static readonly Kind<Controller> Controller = KindBuilder.Build(
			KIND_GROUP_CORE, KIND_VERSION_V1ALPHA1, "controller", "controllers",
			ControllerToDocument, DocumentToController,
			metadata: metadata => metadata
				.RequireString("hostId", format: "uuid"),
			state: state => state
				.OptionalObjectArray("intentFilters", filters => filters
					.OptionalString("action")
					.OptionalString("category"))
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
				new ResourceMetadata(new List<ResourceField>
				{
					new ResourceField("group", ResourceFieldValue.String(kind.Group)),
					new ResourceField("apiVersion", ResourceFieldValue.String(kind.ApiVersion)),
					new ResourceField("name", ResourceFieldValue.String(kind.KindName)),
					new ResourceField("namePlural", ResourceFieldValue.String(kind.KindNamePlural)),
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
			if (resourceDocument.Kind.Group != Kind.Group ||
				resourceDocument.Kind.ApiVersion != Kind.ApiVersion ||
				resourceDocument.Kind.KindName != Kind.KindName)
				return null; //  not a kind

			return new SchemaDrivenKind(
				resourceDocument.Metadata["name"]?.GetString() ?? throw new MissingResourceFieldException("name"),
				resourceDocument.Metadata["namePlural"]?.GetString() ?? throw new MissingResourceFieldException("namePlural"),
				resourceDocument.Metadata["group"]?.GetString() ?? throw new MissingResourceFieldException("group"),
				resourceDocument.Metadata["apiVersion"]?.GetString() ?? throw new MissingResourceFieldException("apiVersion"),
				KindSchema.FromKindSpec(resourceDocument.Spec ?? throw new MissingResourceFieldException("spec")),
				null
				);
		}

		private static ResourceDocument? HostToDocument(Host host)
		{
			return null;
		}

		private static Host? DocumentToHost(ResourceDocument resourceDocument)
		{
			return null;
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
