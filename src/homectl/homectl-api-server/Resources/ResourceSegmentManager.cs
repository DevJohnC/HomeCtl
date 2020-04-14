namespace HomeCtl.ApiServer.Resources
{
	class ResourceSegmentManager
	{
		public readonly ResourceSegmentLibrary MetadataLibrary = new ResourceSegmentLibrary(ResourceSegmentTypes.Metadata);

		public readonly ResourceSegmentLibrary SpecLibrary = new ResourceSegmentLibrary(ResourceSegmentTypes.Spec);

		public readonly ResourceSegmentLibrary StateLibrary = new ResourceSegmentLibrary(ResourceSegmentTypes.State);
	}
}
