using HomeCtl.Kinds;
using HomeCtl.Servers.ApiServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCtl.ApiServer.Hosts
{
	/// <summary>
	/// Manages instances of the host kind which includes managing gRPC connections.
	/// </summary>
	class HostsManager
	{
		private readonly ManagedHost[] _emptyHostCollection = new ManagedHost[0];

		private readonly Dictionary<Guid, ManagedHost> _hosts = new Dictionary<Guid, ManagedHost>();
		private readonly HostsConnectionManager _hostsConnectionManager;

		public HostsManager(HostsConnectionManager hostsConnectionManager)
		{
			_hostsConnectionManager = hostsConnectionManager;
		}

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

		public ApplyResult<ManagedHost> Apply(IEnumerable<ManagedHost> managedHosts)
		{
			List<ManagedHost>? created = null;
			List<ManagedHost>? updated = null;

			foreach (var managedHost in managedHosts)
			{
				if (_hosts.TryGetValue(managedHost.Id, out var storedHost))
				{
					//  update
					if (updated == null)
						updated = new List<ManagedHost>();
					updated.Add(Update(storedHost, managedHost));
				}
				else
				{
					//  create
					if (created == null)
						created = new List<ManagedHost>();
					created.Add(Create(managedHost));
				}
			}

			return new ApplyResult<ManagedHost>(
				created: created,
				updated: updated
				);
		}

		private ManagedHost Create(ManagedHost managedHost)
		{
			_hosts.Add(managedHost.Id, managedHost);
			_hostsConnectionManager.CreateConnectionManager(managedHost);
			return managedHost;
		}

		private ManagedHost Update(ManagedHost currentVersion, ManagedHost newVersion)
		{
			if (currentVersion.Host.State.Endpoint != newVersion.Host.State.Endpoint)
			{
				_hostsConnectionManager.UpdateConnectionManager(newVersion);
			}
			return newVersion;
		}
	}
}
