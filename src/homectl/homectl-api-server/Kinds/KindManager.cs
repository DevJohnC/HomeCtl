using HomeCtl.ApiServer.Resources;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
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

			AddResource(CoreKinds.Kind);
			AddResource(CoreKinds.Host);
			AddResource(CoreKinds.Controller);
			AddResource(CoreKinds.Device);
		}

		protected override Kind<Kind> TypedKind => CoreKinds.Kind;

		protected override Task Created(Kind resource)
		{
			if (!(resource is SchemaDrivenKind schemaDrivenKind))
				return Task.CompletedTask;

			CreateManager(schemaDrivenKind);

			return Task.CompletedTask;
		}

		protected override Kind? CreateFromDocument(ResourceDocument resourceDocument)
		{
			return CoreKinds.DocumentToKind(resourceDocument, ResolveKind);
		}

		private Kind? ResolveKind(KindDescriptor kindDescriptor)
		{
			var key = $"{kindDescriptor.Group}/{kindDescriptor.ApiVersion}/{kindDescriptor.KindName}";
			if (TryGetResource(key, out var kind))
				return kind as Kind;
			return default;
		}

		private GenericKindManager CreateManager(SchemaDrivenKind schemaDrivenKind)
		{
			ResourceManager? extendsManager = null;

			if (schemaDrivenKind.ExtendsKind != null)
			{
				_resourceManagerAccessor.TryFind(q => q.Kind.Group == schemaDrivenKind.ExtendsKind.Group &&
					q.Kind.ApiVersion == schemaDrivenKind.ExtendsKind.ApiVersion &&
					q.Kind.KindName == schemaDrivenKind.ExtendsKind.KindName,
					out extendsManager);
			}

			var manager = new GenericKindManager(
				_documentStoreFactory.CreateDocumentStore(schemaDrivenKind),
				schemaDrivenKind,
				extendsManager
				);

			_resourceManagerAccessor.Add(manager);

			return manager;
		}

		protected override void CopyData(Kind target, Kind source)
		{
			//  no-op for now
			//  updating Kinds isn't supported
			//  but an exception would tell clients that a failure occured
			//  todo: verify target matches source else throw exception
		}

		protected override Task Updated(Kind resource)
		{
			//  no-op for now
			//  updating Kinds isn't supported
			//  exceptional cases handled by CopyData
			return Task.CompletedTask;
		}
	}
}
