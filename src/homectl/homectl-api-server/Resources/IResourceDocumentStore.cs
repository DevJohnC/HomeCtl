using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	interface IResourceDocumentStore<TKind>
		where TKind : class, IResource
	{
		Task Store(string key, ResourceDocument resourceDocument);
	}
}
