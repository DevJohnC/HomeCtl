namespace HomeCtl.ApiServer.Orchestration
{
	/// <summary>
	/// A SpecPatch with accompanying filter for which resources to apply it to.
	/// </summary>
	struct FilteredSpecPatch
	{
		public IResourceFilter ResourceFilter { get; }

		public SpecPatch Patch { get; }
	}

	/// <summary>
	/// Patch for a resource spec.
	/// </summary>
	struct SpecPatch
	{
	}
}
