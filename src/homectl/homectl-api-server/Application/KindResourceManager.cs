using homectl_api_server.Resources;
using Microsoft.OpenApi.Models;

namespace homectl_api_server.Application
{
	/// <summary>
	/// Manages kind resources.
	/// </summary>
	public class KindResourceManager : KindManager
	{
		private readonly ResourceManager _resourceManager;

		public KindResourceManager(ResourceKind kind, ResourceManager resourceManager) :
			base(kind)
		{
			_resourceManager = resourceManager;
		}

		public void Add(ResourceKind kindResource)
		{
			base.Add(kindResource);
		}

		public override Resource Create(ResourceMetadata metadata, ResourceSpec spec)
		{
			var kind = new ResourceKind(string.Empty, string.Empty, string.Empty, ResourceSchema.Nothing);
			var manager = new KindManager(kind);
			_resourceManager.CreateKind(kind, manager);
			return kind;
		}
	}
}
