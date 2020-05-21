using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HomeCtl.ApiServer.Resources
{
	class GenericKindManager : ResourceManager<SchemaDrivenKind.DynamicResource>
	{
		private readonly ResourceManager? _parentKindManager;

		public GenericKindManager(
			IResourceDocumentStore<SchemaDrivenKind.DynamicResource> documentStore,
			Kind<SchemaDrivenKind.DynamicResource> kind,
			ResourceManager? parentKindManager) :
			base(documentStore)
		{
			_parentKindManager = parentKindManager;
			TypedKind = kind;
		}

		protected override Kind<SchemaDrivenKind.DynamicResource> TypedKind { get; }

		protected override bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out string? key)
		{
			key = resourceDocument.Metadata["identifier"]?.GetString();
			return key != null;
		}

		protected override bool TryConvertToResourceInstance(ResourceDocument resourceDocument, [NotNullWhen(true)] out SchemaDrivenKind.DynamicResource? resourceInstance)
		{
			resourceInstance = new SchemaDrivenKind.DynamicResource(Kind, resourceDocument);
			return true;
		}
	}
}
