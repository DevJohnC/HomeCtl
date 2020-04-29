namespace HomeCtl.ApiServer.Resources
{
	public struct ResourceDocumentStoreResult
	{
		public ResourceDocumentStoreResult(bool wasStored, bool updatedExisting)
		{
			WasStored = wasStored;
			UpdatedExisting = updatedExisting;
		}

		public bool WasStored { get; }

		public bool UpdatedExisting { get; }
	}
}
