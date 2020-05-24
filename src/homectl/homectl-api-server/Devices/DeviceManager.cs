using HomeCtl.ApiServer.Resources;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Devices
{
	class DeviceManager : ResourceManager<Device>
	{
		public DeviceManager(IResourceDocumentStore<Device> documentStore) :
			base(documentStore)
		{
		}

		protected override Kind<Device> TypedKind => CoreKinds.Device;

		protected override Task Created(Device resource)
		{
			return Task.CompletedTask;
		}

		protected override Task Loaded(Device resource)
		{
			return Task.CompletedTask;
		}

		protected override Device? CreateFromDocument(ResourceDocument resourceDocument)
		{
			return null;
		}
	}
}
