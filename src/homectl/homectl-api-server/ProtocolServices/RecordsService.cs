using Grpc.Core;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Servers.ApiServer;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class RecordsService : Records.RecordsBase
	{
		private readonly ResourceStore _resourceStore;

		public RecordsService(ResourceStore resourceStore)
		{
			_resourceStore = resourceStore;
		}

		public override Task<StoreRecordResponse> StoreRecord(StoreRecordRequest request, ServerCallContext context)
		{
			var resourceDocument = Resources.ResourceDocument.FromProto(request.ResourceRecord);
			var resource = new Resource(resourceDocument);

			if (_resourceStore.TryStoreResource(resource, out var updatedExisting))
				return Task.FromResult(new StoreRecordResponse
				{
					StoreResult = (updatedExisting) ?
						StoreRecordResponse.Types.StoreResultType.Updated :
						StoreRecordResponse.Types.StoreResultType.Created
				});

			return Task.FromResult(new StoreRecordResponse
			{
				StoreResult = StoreRecordResponse.Types.StoreResultType.Unsaved
			});
		}
	}
}
