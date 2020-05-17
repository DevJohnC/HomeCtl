using HomeCtl.ApiServer.Connections;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	class HostManager : ResourceManager<Guid, Host>
	{
		private readonly ConnectionManager _connectionManager;

		protected override Kind<Host> TypedKind => CoreKinds.Host;

		public HostManager(ConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
		}

		protected override bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out Guid key)
		{
			return Guid.TryParse(resourceDocument.Metadata["hostId"]?.GetString(), out key);
		}

		protected override Task OnResourceCreated(Host resource)
		{
			_connectionManager.CreateConnection(resource);
			return base.OnResourceCreated(resource);
		}

		protected override Task OnResourceUpdated(Host newResource, Host oldResource)
		{
			return base.OnResourceUpdated(newResource, oldResource);
		}
	}
}
