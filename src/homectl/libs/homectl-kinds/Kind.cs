using HomeCtl.Kinds.Resources;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// An instance of a kind.
	/// </summary>
	/// <remarks>
	/// Usage and concept are similar to a Type instance in .NET
	/// </remarks>
	public abstract class Kind : IResource
	{
		public Kind(string kindName, string kindNamePlural, string group, string apiVersion, KindSchema schema, Kind? extendsKind)
		{
			KindName = kindName;
			KindNamePlural = kindNamePlural;
			Group = group;
			ApiVersion = apiVersion;
			Schema = schema;
			ExtendsKind = extendsKind;
		}

		/// <summary>
		/// Gets the kind's name.
		/// </summary>
		public string KindName { get; }

		/// <summary>
		/// Gets the kind's plural name.
		/// </summary>
		public string KindNamePlural { get; }

		/// <summary>
		/// Gets the group the kind belongs to.
		/// </summary>
		public string Group { get; }

		/// <summary>
		/// Gets the api version of the kind.
		/// </summary>
		public string ApiVersion { get; }

		/// <summary>
		/// Gets the schema describing the structure of the kind.
		/// </summary>
		public KindSchema Schema { get; }

		/// <summary>
		/// Gets the kind this kind extends, if it extends a kind.
		/// </summary>
		public Kind? ExtendsKind { get; }

		Kind IResource.Kind => CoreKinds.Kind;

		public KindDescriptor GetKindDescriptor() => new KindDescriptor(Group, ApiVersion, KindName);

		protected abstract bool TryConvertToResourceInstanceImpl(ResourceDocument resourceDocument, [NotNullWhen(true)] out IResource? resourceInstance);

		protected abstract bool TryConvertToDocumentImpl(IResource resourceInstance, [NotNullWhen(true)] out ResourceDocument? resourceDocument);

		public bool TryConvertToResourceInstance(ResourceDocument resourceDocument, [NotNullWhen(true)] out IResource? resourceInstance)
		{
			return TryConvertToResourceInstanceImpl(resourceDocument, out resourceInstance);
		}

		public bool TryConvertToDocument(IResource resourceInstance, [NotNullWhen(true)] out ResourceDocument? resourceDocument)
		{
			return TryConvertToDocumentImpl(resourceInstance, out resourceDocument);
		}

		public string GetIdentity()
		{
			return $"{Group}/{ApiVersion}/{KindName}";
		}
	}

	public class Kind<T> : Kind
		where T : class, IResource
	{
		private readonly Func<T, ResourceDocument?> _convertToDocument;
		private readonly Func<ResourceDocument, T?> _convertToResource;

		public Kind(string kindName, string kindNamePlural, string group, string apiVersion,
			KindSchema schema, Kind? extendsKind, Func<T, ResourceDocument?> convertToDocument,
			Func<ResourceDocument, T?> convertToResource) :
			base(kindName, kindNamePlural, group, apiVersion, schema, extendsKind)
		{
			_convertToDocument = convertToDocument;
			_convertToResource = convertToResource;
		}

		protected override bool TryConvertToResourceInstanceImpl(ResourceDocument resourceDocument, [NotNullWhen(true)]  out IResource? resourceInstance)
		{
			var result = TryConvertToResourceInstance(resourceDocument, out T? typedResourceInstance);
			resourceInstance = typedResourceInstance;
			return result;
		}

		protected override bool TryConvertToDocumentImpl(IResource resourceInstance, [NotNullWhen(true)]  out ResourceDocument? resourceDocument)
		{
			var typedInstance = resourceInstance as T;
			if (typedInstance == null)
			{
				resourceDocument = default;
				return false;
			}

			return TryConvertToDocument(typedInstance, out resourceDocument);
		}

		public bool TryConvertToDocument(T resourceInstance, [NotNullWhen(true)]  out ResourceDocument? resourceDocument)
		{
			try
			{
				resourceDocument = _convertToDocument(resourceInstance);
				return resourceDocument != null;
			}
			catch
			{
				resourceDocument = null;
				return false;
			}
		}

		public bool TryConvertToResourceInstance(ResourceDocument resourceDocument, [NotNullWhen(true)]  out T? resourceInstance)
		{
			try
			{
				resourceInstance = _convertToResource(resourceDocument);
				return resourceInstance != null;
			}
			catch
			{
				resourceInstance = default;
				return false;
			}
		}
	}

	public class SchemaDrivenKind : Kind<SchemaDrivenKind.DynamicResource>
	{
		public SchemaDrivenKind(string kindName, string kindNamePlural, string group, string apiVersion, KindSchema schema, Kind? extendsKind) :
			base(kindName, kindNamePlural, group, apiVersion, schema, extendsKind, ConvertToDocument, ConvertToResource)
		{
		}

		private static ResourceDocument? ConvertToDocument(DynamicResource dynamicResource)
		{
			return dynamicResource.ResourceDocument;
		}

		private static DynamicResource? ConvertToResource(ResourceDocument resourceDocument)
		{
			return null;
		}

		public class DynamicResource : IResource
		{
			public DynamicResource(Kind kind, ResourceDocument resourceDocument)
			{
				Kind = kind;
				ResourceDocument = resourceDocument;
			}

			public Kind Kind { get; }

			public ResourceDocument ResourceDocument { get; }

			public string GetIdentity()
			{
				return ResourceDocument.Definition["identity"]?.GetString() ??
					throw new Exception("Missing identity.");
			}
		}
	}
}
