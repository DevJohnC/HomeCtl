using Grpc.Core;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Services.Server;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class ControlService : Control.ControlBase
	{
		private readonly ResourceOrchestrator _resourceOrchestrator;

		public ControlService(ResourceOrchestrator resourceOrchestrator)
		{
			_resourceOrchestrator = resourceOrchestrator;
		}

		public override async Task<ApplyDocumentResponse> ApplyDocument(ApplyDocumentRequest request, ServerCallContext context)
		{
			var resourceDocument = request.ResourceDocument.ToResourceDocument();
			await _resourceOrchestrator.Apply(resourceDocument);
			return new ApplyDocumentResponse();
		}
	}
}
