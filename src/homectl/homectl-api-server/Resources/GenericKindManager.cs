using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

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

		protected override Task Created(SchemaDrivenKind.DynamicResource resource)
		{
			return Task.CompletedTask;
		}

		protected override Task Loaded(SchemaDrivenKind.DynamicResource resource)
		{
			return Task.CompletedTask;
		}

		protected override SchemaDrivenKind.DynamicResource? CreateFromDocument(ResourceDocument resourceDocument)
		{
			return new SchemaDrivenKind.DynamicResource(Kind, resourceDocument);
		}
	}
}
