using Newtonsoft.Json.Linq;

namespace homectl.Resources
{
	public class HostResource : IResource<HostResource.HostMetadata, HostResource.HostSpec, HostResource.HostState>
	{
		public HostState State { get; set; }

		public HostMetadata Metadata { get; set; }

		public HostSpec Spec { get; set; }

		public class HostMetadata
		{
			public string Hostname { get; set; }

			public static HostMetadata FromJson(JObject json)
			{
				return new HostMetadata
				{
					Hostname = json.Value<string>("hostname")
				};
			}
		}

		public class HostSpec
		{
			public string Endpoint { get; set; }

			public static HostSpec FromJson(JObject json)
			{
				return new HostSpec
				{
					Endpoint = json.Value<string>("endpoint")
				};
			}
		}

		public class HostState
		{
			public HostStatus Status { get; set; }
		}

		public enum HostStatus
		{
			Disconnected,
			Connecting,
			Connected
		}
	}
}
