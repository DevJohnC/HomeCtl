using homectl_api_server.Resources;

namespace homectl_api_server.Application
{
	public class NodeResourceManager : KindManager
	{
		public NodeResourceManager(ResourceKind kind) :
			base(kind)
		{
		}

		public override Resource Create(ResourceMetadata metadata, ResourceSpec spec)
		{
			var node = new Node(metadata, spec, ResourceState.Nothing);
			Add(node);
			return node;
		}
	}
}
