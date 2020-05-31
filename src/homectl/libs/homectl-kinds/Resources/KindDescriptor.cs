namespace HomeCtl.Kinds.Resources
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

		public override bool Equals(object obj)
		{
			if (obj is KindDescriptor other)
				return other.Group == Group &&
					other.ApiVersion == ApiVersion &&
					other.KindName == KindName;
			return false;
		}
	}
}
