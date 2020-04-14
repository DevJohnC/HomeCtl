using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Orchestration
{
	/// <summary>
	/// Applies spec changes to resources.
	/// </summary>
	interface ISpecApplier
	{
		/// <summary>
		/// Apply spec changes to all matching resources, if possible.
		/// </summary>
		/// <param name="specPatch"></param>
		/// <returns></returns>
		Task<object> ApplySpecChanges(FilteredSpecPatch specPatch);
	}
}
