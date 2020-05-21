using HomeCtl.ApiServer.Resources;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Diagnostics.CodeAnalysis;

namespace HomeCtl.ApiServer.Devices
{
	class DeviceManager : ResourceManager<Device>
	{
		public DeviceManager(IResourceDocumentStore<Device> documentStore) :
			base(documentStore)
		{
		}

		protected override Kind<Device> TypedKind => CoreKinds.Device;

		protected override bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out string? key)
		{
			throw new System.NotImplementedException();
		}
	}
}
