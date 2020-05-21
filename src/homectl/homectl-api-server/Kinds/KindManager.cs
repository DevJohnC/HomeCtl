using HomeCtl.ApiServer.Resources;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Kinds
{
	class KindManager : ResourceManager<Kind>
	{
		private readonly IDocumentStoreFactory _documentStoreFactory;
		private readonly ResourceManagerAccessor _resourceManagerAccessor;

		public KindManager(IResourceDocumentStore<Kind> documentStore,
			IDocumentStoreFactory documentStoreFactory,
			ResourceManagerAccessor resourceManagerAccessor) :
			base(documentStore)
		{
			_documentStoreFactory = documentStoreFactory;
			_resourceManagerAccessor = resourceManagerAccessor;
			AddCoreKind(CoreKinds.Kind);
			AddCoreKind(CoreKinds.Host);
			AddCoreKind(CoreKinds.Controller);
			AddCoreKind(CoreKinds.Device);
		}

		private void AddCoreKind(Kind kind)
		{
			Add($"{kind.Group}-{kind.ApiVersion}-{kind.KindName}", kind);
		}

		protected override Kind<Kind> TypedKind => CoreKinds.Kind;

		protected override Task OnResourceLoaded(Kind resource)
		{
			if (!(resource is SchemaDrivenKind schemaDrivenKind))
				return Task.CompletedTask;

			ResourceManager? extendsManager = null;

			if (resource.ExtendsKind != null)
			{
				_resourceManagerAccessor.TryFind(q => q.Kind.Group == resource.ExtendsKind.Group &&
					q.Kind.ApiVersion == resource.ExtendsKind.ApiVersion &&
					q.Kind.KindName == resource.ExtendsKind.KindName,
					out extendsManager);
			}

			var manager = new GenericKindManager(
				_documentStoreFactory.CreateDocumentStore(schemaDrivenKind),
				schemaDrivenKind,
				extendsManager
				);

			_resourceManagerAccessor.Orchestrator.AddResourceManager(manager);

			return manager.LoadResources();
		}

		protected override bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out string? key)
		{
			key = $"{resourceDocument.Metadata["group"].GetString()}-{resourceDocument.Metadata["apiVersion"].GetString()}-{resourceDocument.Metadata["name"].GetString()}";
			return true;
		}

		protected override bool TryConvertToResourceInstance(ResourceDocument resourceDocument, [NotNullWhen(true)] out Kind? resourceInstance)
		{
			var kind = CoreKinds.DocumentToKind(resourceDocument, ResolveKind);
			if (kind == null)
			{
				resourceInstance = default;
				return false;
			}

			resourceInstance = kind;
			return true;
		}

		private Kind? ResolveKind(KindDescriptor kindDescriptor)
		{
			var key = $"{kindDescriptor.Group}-{kindDescriptor.ApiVersion}-{kindDescriptor.KindName}";
			if (TryGetResource(key, out var kind))
				return kind;
			return default;
		}
	}
}
