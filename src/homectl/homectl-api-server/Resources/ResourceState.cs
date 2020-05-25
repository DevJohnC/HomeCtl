using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;

namespace HomeCtl.ApiServer.Resources
{
	struct ResourceState
	{
		public ResourceState(ResourceManager manager, string identity, ResourceDocument fullDocument)
		{
			Manager = manager;
			Identity = identity;
			FullDocument = fullDocument;
		}

		public ResourceManager Manager { get; }

		public Kind Kind => Manager.Kind;

		public string Identity { get; }

		public ResourceDocument FullDocument { get; }

	}
}
