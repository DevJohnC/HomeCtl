using HomeCtl.Kinds;
using System.Collections.Generic;
using System.IO;

namespace HomeCtl.Host
{
	public class HostOptions
	{
		public int HostPort { get; internal set; }

		public List<Kind> Kinds { get; } = new List<Kind>();

		public string? HostFile { get; set; }

		public AppHost GetAppHost()
		{
			if (string.IsNullOrWhiteSpace(HostFile))
				return AppHost.FromLocalEnvironment();

			AppHost host;
			if (!File.Exists(HostFile))
				host = AppHost.FromLocalEnvironment();
			else
				host = AppHost.FromFile(HostFile);

			host.SaveToFile(HostFile);

			return host;
		}
	}
}
