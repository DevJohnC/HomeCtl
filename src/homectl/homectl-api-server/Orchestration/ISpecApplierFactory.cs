namespace HomeCtl.ApiServer.Orchestration
{
	/// <summary>
	/// Provides instances of ISpecApplier that attempt apply the changes in a spec to a resource.
	/// </summary>
	interface ISpecApplierFactory
	{
		/// <summary>
		/// Gets an applier.
		/// </summary>
		/// <returns></returns>
		ISpecApplier GetApplier();
	}
}
