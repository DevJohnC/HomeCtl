using HomeCtl.Kinds;

namespace HomeCtl.ApiServer.Resources
{
	interface IDocumentStoreFactory
	{
		public IResourceDocumentStore<T> CreateDocumentStore<T>(Kind<T> kind)
			where T : class, IResource;
	}
}
