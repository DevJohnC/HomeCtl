using Grpc.Core;
using HomeCtl.ApiServer.Hosts;
using HomeCtl.Servers.ApiServer;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class HostsService : Servers.ApiServer.Hosts.HostsBase
	{
		private readonly HostsManager _hostsManager;

		public HostsService(HostsManager hostsManager)
		{
			_hostsManager = hostsManager;
		}

		public override Task<HostStoreResponse> Store(HostStoreRequest request, ServerCallContext context)
		{
			var remoteHostname = context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "localhost";
			var metadata = Kinds.Host.HostMetadata.FromJson(JObject.Parse(request.HostManifest.MetadataJson));
			var managedHosts = _hostsManager.FindMatchingHosts(request.HostMatchQuery);

			var newId = Guid.NewGuid();
			if (!string.IsNullOrWhiteSpace(request.HostManifest.HostId))
			{
				if (!Guid.TryParse(request.HostManifest.HostId, out newId))
					throw new Exception("Malformed host id.");
			}

			if (managedHosts.Count == 0)
			{
				managedHosts = new ManagedHost[]
				{
					new ManagedHost(
						newId,
						new Kinds.Host(
							metadata,
							new Kinds.Host.HostState(
								$"{(request.HostManifest.EndpointType == HostEndpointThpe.Http ? "http" : "https")}://{remoteHostname}:{request.HostManifest.EndpointPort}",
								Kinds.Host.HostStatus.Disconnected)))
				};
				var applyResult = _hostsManager.Apply(managedHosts);

				return Task.FromResult(CreateResponse(applyResult));
			}
			else
			{
				var applyResult = _hostsManager.Apply(managedHosts.Select(managedHost =>
					managedHost.WithHost(
						managedHost.Host.WithMetadata(metadata))));

				return Task.FromResult(CreateResponse(applyResult));
			}
		}

		private HostStoreResponse CreateResponse(ApplyResult<ManagedHost> applyResult)
		{
			var response = new HostStoreResponse();

			foreach (var managedHost in applyResult.Created)
			{
				response.Created.Add(new HostStoreRecord
				{
					HostId = managedHost.Id.ToString(),
					MetadataJson = managedHost.Host.Metadata.ToString(),
					StateJson = managedHost.Host.State.ToString()
				});
			}

			foreach (var managedHost in applyResult.Updated)
			{
				response.Updated.Add(new HostStoreRecord
				{
					HostId = managedHost.Id.ToString(),
					MetadataJson = managedHost.Host.Metadata.ToString(),
					StateJson = managedHost.Host.State.ToString()
				});
			}

			return response;
		}
	}
}
