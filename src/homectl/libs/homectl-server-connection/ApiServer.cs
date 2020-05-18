using HomeCtl.Events;
using HomeCtl.Kinds;
using HomeCtl.Services;
using HomeCtl.Services.Server;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public class ApiServer : Server
	{
		public ApiServer(EndpointConnectionManager connectionManager, EventBus eventBus,
			ILogger<ApiServer> logger) :
			base(connectionManager, eventBus, logger)
		{
		}

		public Task Apply(IResource resource)
		{
			if (!resource.Kind.TryConvertToDocument(resource, out var resourceDocument))
				throw new System.Exception("Failed to convert resource to document.");

			return Apply(resourceDocument);
		}

		public async Task Apply(Kinds.Resources.ResourceDocument resourceDocument)
		{
			var protoResourceDocument = ResourceDocument.FromResourceDocument(resourceDocument);
			var client = new Control.ControlClient(ConnectionManager.ServicesChannel);
			await client.ApplyDocumentAsync(new ApplyDocumentRequest
			{
				ResourceDocument = protoResourceDocument
			});
		}
	}
}
