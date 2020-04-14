namespace HomeCtl.ApiServer.Resources
{
	/// <summary>
	/// Manages one segment type for resource instances.
	/// </summary>
	class ResourceSegmentLibrary
	{
		public ResourceSegmentLibrary(string segmentType)
		{
			SegmentType = segmentType;
		}

		public string SegmentType { get; }
	}

	static class ResourceSegmentTypes
	{
		public static readonly string Metadata = "metadata";
		public static readonly string Spec = "spec";
		public static readonly string State = "state";
	}
}
