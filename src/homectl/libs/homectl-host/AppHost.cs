using HomeCtl.Kinds;
using System;

namespace HomeCtl.Host
{
	public class AppHost : Kinds.Host
	{
		public AppHost()
		{
		}

		public AppHost(Kinds.Host copy)
		{
			HostId = copy.HostId;
			Endpoint = copy.Endpoint;
			MachineName = copy.MachineName;
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
			return new AppHost
			{
				HostId = hostId,
				MachineName = Environment.MachineName
			};
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
