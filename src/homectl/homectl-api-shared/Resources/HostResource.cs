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
		}

		public class HostSpec
		{
			public string Endpoint { get; set; }
		}

		public class HostState
		{
			public HostStatus Status { get; set; }
		}

		public enum HostStatus
		{
			Connected,
			Connecting,
			Disconnected
		}
	}
}
