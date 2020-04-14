using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Orchestration
{
	/// <summary>
	/// An ISpecApplierFactory implementation that provides ISpecApplier instances that are
	/// capable of orchestrating spec changes on networked hosts.
	/// </summary>
	class NetworkedSpecApplierFactory : ISpecApplierFactory
	{
		public ISpecApplier GetApplier()
		{
			return new NetworkSpecApplier();
		}

		private class NetworkSpecApplier : ISpecApplier
		{
			public Task<object> ApplySpecChanges(FilteredSpecPatch specPatch)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
