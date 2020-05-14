using System;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// A host running somewhere on the network.
	/// </summary>
	public class Host : IResource
	{
		public Host(HostMetadata metadata, HostState state)
		{
			Metadata = metadata;
			State = state;
		}

		public Kind Kind => CoreKinds.Host;

		public HostMetadata Metadata { get; set; }

		public HostState State { get; set; }

		public class HostMetadata
		{
			public Guid HostId { get; set; }

			public string MachineName { get; set; } = "";
		}

		public class HostState
		{
			public string Endpoint { get; set; } = "";

			public ConnectedState ConnectedState { get; set; }
		}

		public enum ConnectedState
		{
			NotConnected,
			Connected
		}
	}
}
