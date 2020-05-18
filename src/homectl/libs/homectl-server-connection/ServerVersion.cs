namespace HomeCtl.Connection
{
	public struct ServerVersion
	{
		public ServerVersion(int major, int minor, string name)
		{
			Major = major;
			Minor = minor;
			Name = name;
		}

		public int Major { get; }
		public int Minor { get; }
		public string Name { get; }

		public override string ToString()
		{
			return $"{Major}.{Minor}-{Name}";
		}
	}
}
