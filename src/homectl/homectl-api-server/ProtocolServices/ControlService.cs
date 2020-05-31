using Grpc.Core;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Services.Server;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class ControlService : Control.ControlBase
	{
		private readonly ResourceStateStore _resourceOrchestrator;

		public ControlService(ResourceStateStore resourceOrchestrator)
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
