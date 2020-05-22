using System;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// A host running somewhere on the network.
	/// </summary>
	public class Host : IResource
	{
		public Kind Kind => CoreKinds.Host;

		public Guid HostId { get; set; }

		public string MachineName { get; set; } = "";

		public string Endpoint { get; set; } = "";

		public enum ConnectedState
		{
			NotConnected,
			Connected
		}
	}
}
