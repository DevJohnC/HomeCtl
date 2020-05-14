namespace HomeCtl.Connection
{
	public struct ApiServerVersion
	{
		public ApiServerVersion(int major, int minor, string name)
		{
			Major = major;
			Minor = minor;
			Name = name;
		}

		public int Major { get; }
		public int Minor { get; }
		public string Name { get; }
	}
}
