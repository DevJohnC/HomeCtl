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
			this(HostMetadata.FromJson(metadata), new HostState(string.Empty))
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

		public enum ConnectedState
		{
			Online,
			Offline
		}

		/// <summary>
		/// Metadata structure for hosts.
		/// </summary>
		public struct HostMetadata
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
					json.Value<string>("name") ?? "",
					json.Value<string>("hostname") ?? ""
					);
			}
		}

		/// <summary>
		/// State structure for hosts.
		/// </summary>
		public struct HostState
		{
			public HostState(string endpoint)
			{
				Endpoint = endpoint;
			}

			/// <summary>
			/// Gets or sets the gRPC endpoint the host is available on.
			/// </summary>
			public string Endpoint { get; set; }
		}
	}
}
