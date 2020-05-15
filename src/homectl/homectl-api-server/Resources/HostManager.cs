using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HomeCtl.ApiServer.Resources
{
	class HostManager : ResourceManager<Guid, Host>
	{
		protected override Kind<Host> TypedKind => CoreKinds.Host;

		protected override bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out Guid key)
		{
			return Guid.TryParse(resourceDocument.Metadata["hostId"]?.GetString(), out key);
		}
	}
}
