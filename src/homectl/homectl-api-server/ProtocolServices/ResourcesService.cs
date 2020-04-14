using Grpc.Core;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Servers.ApiServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class ResourcesService : Servers.ApiServer.Resources.ResourcesBase
	{
		private readonly ResourceManager _resourceManager;
		private readonly JsonSerializer _jsonSerializer;

		public ResourcesService(ResourceManager resourceManager, JsonSerializer jsonSerializer)
		{
			_resourceManager = resourceManager;
			_jsonSerializer = jsonSerializer;
		}

		private JObject DeserializeJsonString(string jsonString)
		{
			if (string.IsNullOrWhiteSpace(jsonString))
				return new JObject();

			using (var textReader = new StringReader(jsonString))
			using (var jsonReader = new JsonTextReader(textReader))
			{
				return _jsonSerializer.Deserialize<JObject>(jsonReader) ?? new JObject();
			}
		}

		public override Task<ResourceStoreResponse> Create(ResourceStoreRequest request, ServerCallContext context)
		{
			var resourceId = Guid.Empty;

			var metadataJson = DeserializeJsonString(request.MetadataJson);
			var specJson = DeserializeJsonString(request.SpecJson);

			if (metadataJson.ContainsKey("Id"))
			{
				if (!Guid.TryParse(metadataJson.Value<string>("Id"), out var parsedId))
				{
					//  malformed id
					return Task.FromResult(new ResourceStoreResponse
					{
						KindIdentifier = request.KindIdentifier,
						StoreResult = ResourceStoreResponse.Types.StoreResultType.Unsaved
					});
				}
				resourceId = parsedId;
			}
			else
			{
				resourceId = Guid.NewGuid();
			}

			if (_resourceManager.TryGetResource(
				request.KindIdentifier.KindGroup, request.KindIdentifier.KindApiVersion,
				request.KindIdentifier.KindName, resourceId, out var _
				))
			{
				return Task.FromResult(new ResourceStoreResponse
				{
					KindIdentifier = request.KindIdentifier,
					StoreResult = ResourceStoreResponse.Types.StoreResultType.Unsaved
				});
			}

			return base.Create(request, context);
		}

		public override Task<ResourceStoreResponse> Store(ResourceStoreRequest request, ServerCallContext context)
		{
			return base.Store(request, context);
		}
	}
}
