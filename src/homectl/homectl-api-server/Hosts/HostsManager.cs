using HomeCtl.Kinds;
using HomeCtl.Servers.ApiServer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCtl.ApiServer.Hosts
{
	/// <summary>
	/// Manages instances of the host kind which includes managing gRPC connections.
	/// </summary>
	public class HostsManager
	{
		private readonly ManagedHost[] _emptyHostCollection = new ManagedHost[0];

		private readonly Dictionary<Guid, ManagedHost> _hosts = new Dictionary<Guid, ManagedHost>();

		/// <summary>
		/// Get all managed hosts that match the provided query.
		/// </summary>
		/// <param name="recordQuery"></param>
		/// <returns></returns>
		public IReadOnlyList<ManagedHost> FindMatchingHosts(RecordQuery recordQuery)
		{
			switch(recordQuery.QueryCase)
			{
				case RecordQuery.QueryOneofCase.ResourceId:
					if (!Guid.TryParse(recordQuery.ResourceId, out var id))
					{
						return _emptyHostCollection;
					}
					if (!_hosts.TryGetValue(id, out var managedHost))
					{
						return _emptyHostCollection;
					}
					return new[] { managedHost };
				case RecordQuery.QueryOneofCase.MetadataQuery:
					List<ManagedHost>? results = null;
					foreach (var host in _hosts.Values.Where(
						q => q.Host.MatchesQuery(recordQuery.MetadataQuery)
					))
					{
						if (results == null)
							results = new List<ManagedHost>();
						results.Add(host);
					}
					return results ?? (IReadOnlyList<ManagedHost>)_emptyHostCollection;
				default:
					return _emptyHostCollection;
			}
		}

		//public bool TryCreateHost(HostStoreRequest storeRequest, string remoteHostname, out ManagedHost? managedHost)
		//{
		//	managedHost = new ManagedHost(
		//		Guid.Parse(storeRequest.HostManifest.HostId),
		//		new Host(
		//			Host.HostMetadata.FromJson(JObject.Parse(storeRequest.HostManifest.MetadataJson)),
		//			new Host.HostState(
		//				$"{(storeRequest.HostManifest.EndpointType == HostEndpointThpe.Http ? "http" : "https")}://{remoteHostname}:{storeRequest.HostManifest.EndpointPort}",
		//				Host.HostStatus.Disconnected
		//				)));
		//	return true;
		//}
	}
}
