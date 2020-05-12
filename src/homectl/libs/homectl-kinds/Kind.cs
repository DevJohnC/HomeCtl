using HomeCtl.Kinds.Resources;
using System;

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

		protected abstract bool TryConvertToResourceInstanceImpl(ResourceDocument resourceDocument, out IResource? resourceInstance);

		protected abstract bool TryConvertToDocumentImpl(IResource resourceInstance, out ResourceDocument? resourceDocument);

		public bool TryConvertToResourceInstance(ResourceDocument resourceDocument, out IResource? resourceInstance)
		{
			return TryConvertToResourceInstanceImpl(resourceDocument, out resourceInstance);
		}

		public bool ConvertToDocument(IResource resourceInstance, out ResourceDocument? resourceDocument)
		{
			return TryConvertToDocumentImpl(resourceInstance, out resourceDocument);
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

		protected override bool TryConvertToResourceInstanceImpl(ResourceDocument resourceDocument, out IResource? resourceInstance)
		{
			var result = TryConvertToResourceInstance(resourceDocument, out T? typedResourceInstance);
			resourceInstance = typedResourceInstance;
			return result;
		}

		protected override bool TryConvertToDocumentImpl(IResource resourceInstance, out ResourceDocument? resourceDocument)
		{
			var typedInstance = resourceInstance as T;
			if (typedInstance == null)
			{
				resourceDocument = default;
				return false;
			}

			return TryConvertToDocument(typedInstance, out resourceDocument);
		}

		public bool TryConvertToDocument(T resourceInstance, out ResourceDocument? resourceDocument)
		{
			try
			{
				resourceDocument = _convertToDocument(resourceInstance);
				return true;
			}
			catch
			{
				resourceDocument = null;
				return false;
			}
		}

		public bool TryConvertToResourceInstance(ResourceDocument resourceDocument, out T? resourceInstance)
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

	public class SchemaDrivenKind : Kind
	{
		public SchemaDrivenKind(string kindName, string kindNamePlural, string group, string apiVersion, KindSchema schema, Kind? extendsKind) :
			base(kindName, kindNamePlural, group, apiVersion, schema, extendsKind)
		{
		}

		protected override bool TryConvertToDocumentImpl(IResource resourceInstance, out ResourceDocument? resourceDocument)
		{
			throw new NotImplementedException();
		}

		protected override bool TryConvertToResourceInstanceImpl(ResourceDocument resourceDocument, out IResource? resourceInstance)
		{
			throw new NotImplementedException();
		}
	}
}
