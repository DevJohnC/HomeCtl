using Grpc.Core;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Services.Server;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class ControlService : Control.ControlBase
	{
		private ResourceDocumentStore _documentStore;
		private ResourceManager _resourceManager;

		public ControlService(ResourceDocumentStore documentStore, ResourceManager resourceManager)
		{
			_documentStore = documentStore;
			_resourceManager = resourceManager;
		}

		public override async Task<ApplyDocumentResponse> ApplyDocument(ApplyDocumentRequest request, ServerCallContext context)
		{
			var resourceDocument = request.ResourceDocument.ToResourceDocument();

			var storeResult = await _documentStore.StoreResourceDocument(resourceDocument);
			if (!storeResult.WasStored)
				return new ApplyDocumentResponse();

			_resourceManager.ApplyChanges(storeResult);

			return new ApplyDocumentResponse();
		}
	}
}
