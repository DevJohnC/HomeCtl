using HomeCtl.Kinds;
using System;

namespace HomeCtl.Host
{
	public class AppHost : Kinds.Host
	{
		public AppHost(HostMetadata metadata, HostState state) :
			base(metadata, state)
		{
		}

		public AppHost(Kinds.Host copy) :
			this(copy.Metadata, copy.State)
		{
		}

		public void SaveToFile(string filePath)
		{
			Resources.Resources.SaveToFile(this, filePath);
		}

		public static AppHost FromLocalEnvironment()
		{
			return FromLocalEnvironment(Guid.NewGuid());
		}

		public static AppHost FromLocalEnvironment(Guid hostId)
		{
			return new AppHost(
				new HostMetadata { HostId = hostId, MachineName = Environment.MachineName },
				new HostState { }
				);
		}

		public static AppHost FromFile(string filePath)
		{
			var host = Resources.Resources.LoadFromFile(CoreKinds.Host, filePath);
			if (host == null)
				throw new Exception("Failed to load host resource.");
			return new AppHost(host);
		}
	}
}
