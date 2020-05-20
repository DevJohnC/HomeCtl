using HomeCtl.ApiServer.Resources;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Diagnostics.CodeAnalysis;

namespace HomeCtl.ApiServer.Kinds
{
	class KindManager : ResourceManager<Kind>
	{
		public KindManager(IResourceDocumentStore<Kind> documentStore) :
			base(documentStore)
		{
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
