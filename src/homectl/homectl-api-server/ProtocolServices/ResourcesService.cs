using Grpc.Core;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Clients;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class ResourcesService : Clients.Resources.ResourcesBase
	{
		private readonly ResourceStore _resourceStore;

		public ResourcesService(ResourceStore resourceStore)
		{
			_resourceStore = resourceStore;
		}

		public override Task<ResourceStoreResponse> Create(ResourceStoreRequest request, ServerCallContext context)
		{
			return base.Create(request, context);
		}

		public override Task<ResourceStoreResponse> Store(ResourceStoreRequest request, ServerCallContext context)
		{
			return base.Store(request, context);
		}
	}
}
