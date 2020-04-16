using Grpc.Core;
using HomeCtl.ApiServer.Resources;
using HomeCtl.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class ResourcesService : Clients.Resources.ResourcesBase
	{
		private readonly ResourceStore _resourceStore;
		private readonly JsonSerializer _jsonSerializer;

		public ResourcesService(ResourceStore resourceStore, JsonSerializer jsonSerializer)
		{
			_resourceStore = resourceStore;
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

		private string SerializeToJsonString(JObject jsonObject)
		{
			using (var textWriter = new StringWriter())
			using (var jsonWriter = new JsonTextWriter(textWriter))
			{
				_jsonSerializer.Serialize(jsonWriter, jsonObject);
				return textWriter.ToString();
			}
		}

		public override Task<ResourceStoreResponse> Create(ResourceStoreRequest request, ServerCallContext context)
		{
			var resourceId = Guid.Empty;

			var kind = new KindDescriptor(
				request.KindIdentifier.KindGroup, request.KindIdentifier.KindApiVersion,
				request.KindIdentifier.KindName
				);
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
						StoreResult = ResourceStoreResponse.Types.StoreResultType.Unsaved,
						MetadataJson = request.MetadataJson,
						SpecJson = request.SpecJson
					});
				}
				resourceId = parsedId;
			}
			else
			{
				resourceId = Guid.NewGuid();
			}

			if (_resourceStore.TryGetResource(new ResourceRecord(resourceId), kind, out var _
				))
			{
				//  resource with same id already exists
				return Task.FromResult(new ResourceStoreResponse
				{
					KindIdentifier = request.KindIdentifier,
					StoreResult = ResourceStoreResponse.Types.StoreResultType.Unsaved,
					MetadataJson = request.MetadataJson,
					SpecJson = request.SpecJson
				});
			}

			var resource = new Resource(
				new ResourceRecord(resourceId, metadataJson.Value<string>("Label")),
				kind, metadataJson, specJson);
			if (!_resourceStore.TryStoreResource(resource, out var storedVersion))
			{
				//  failure, likely kind doesnt exist or data isnt valid
				return Task.FromResult(new ResourceStoreResponse
				{
					KindIdentifier = request.KindIdentifier,
					StoreResult = ResourceStoreResponse.Types.StoreResultType.Unsaved,
					MetadataJson = request.MetadataJson,
					SpecJson = request.SpecJson
				});
			}

			return Task.FromResult(new ResourceStoreResponse
			{
				KindIdentifier = request.KindIdentifier,
				StoreResult = ResourceStoreResponse.Types.StoreResultType.Created,
				MetadataJson = SerializeToJsonString(storedVersion.MetadataJson),
				SpecJson = SerializeToJsonString(storedVersion.SpecJson)
			});
		}

		public override Task<ResourceStoreResponse> Store(ResourceStoreRequest request, ServerCallContext context)
		{
			return base.Store(request, context);
		}
	}
}
