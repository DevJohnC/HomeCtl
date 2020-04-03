using Newtonsoft.Json.Linq;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// A host running somewhere on the network.
	/// </summary>
	public class Host : IResource
	{
		public Host(HostMetadata metadata, HostState state)
		{
			State = state;
			Metadata = metadata;
		}

		public Host(JObject metadata) :
			this(HostMetadata.FromJson(metadata), new HostState(string.Empty, HostStatus.Disconnected))
		{
		}

		public Kind Kind => CoreKinds.Host;

		/// <summary>
		/// Gets or sets the current known state of the host.
		/// </summary>
		public HostState State { get; set; }

		/// <summary>
		/// Gets or sets the metadata of the host.
		/// </summary>
		public HostMetadata Metadata { get; set; }

		/// <summary>
		/// Metadata structure for hosts.
		/// </summary>
		public class HostMetadata
		{
			public HostMetadata(string name, string hostname)
			{
				Name = name;
				Hostname = hostname;
			}

			/// <summary>
			/// Gets or sets a freeform name for the host.
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// Gets or sets the machine name of the computer the host is running on.
			/// </summary>
			public string Hostname { get; set; }

			/// <summary>
			/// Create an instance from a json structure.
			/// </summary>
			/// <param name="json"></param>
			/// <returns></returns>
			public static HostMetadata FromJson(JObject json)
			{
				return new HostMetadata(
					json.Value<string>("name"),
					json.Value<string>("hostname")
					);
			}
		}

		/// <summary>
		/// State structure for hosts.
		/// </summary>
		public class HostState
		{
			public HostState(string endpoint, HostStatus status)
			{
				Endpoint = endpoint;
				Status = status;
			}

			/// <summary>
			/// Gets or sets the gRPC endpoint the host is available on.
			/// </summary>
			public string Endpoint { get; set; }

			/// <summary>
			/// Gets or sets the current availability status of the host.
			/// </summary>
			public HostStatus Status { get; set; }
		}

		/// <summary>
		/// Host availability status.
		/// </summary>
		public enum HostStatus
		{
			Disconnected,
			Connecting,
			Connected
		}
	}
}
