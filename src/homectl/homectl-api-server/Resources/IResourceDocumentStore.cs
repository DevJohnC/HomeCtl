using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	interface IResourceDocumentStore<TKind>
		where TKind : class, IResource
	{
		Task<IReadOnlyList<ResourceDocument>> LoadAll();

		Task Store(string key, ResourceDocument resourceDocument);
	}
}
