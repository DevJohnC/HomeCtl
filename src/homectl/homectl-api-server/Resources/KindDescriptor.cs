namespace HomeCtl.ApiServer.Resources
{
	public struct KindDescriptor
	{
		public KindDescriptor(string group, string apiVersion, string kindName)
		{
			Group = group;
			ApiVersion = apiVersion;
			KindName = kindName;
		}

		public string Group { get; }

		public string ApiVersion { get; }

		public string KindName { get; }
	}
}
