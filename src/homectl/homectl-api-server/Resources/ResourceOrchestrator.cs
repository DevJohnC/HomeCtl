using HomeCtl.Kinds.Resources;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceOrchestrator
	{
		private readonly ResourceManagerCollection _resourceManagers = new ResourceManagerCollection();

		public ResourceOrchestrator(IEnumerable<ResourceManager> coreResourceManagers)
		{
			_resourceManagers.AddRange(coreResourceManagers);
		}

		public Task Apply(ResourceDocument resourceDocument)
		{
			if (!_resourceManagers.TryGetResourceManager(resourceDocument.Kind, out var resourceManager))
			{
				return Task.CompletedTask;
			}

			//  todo: validate the document

			if (!resourceManager.TryGetResource(resourceDocument, out var resource))
			{
				return resourceManager.CreateResource(resourceDocument);
			}

			return resourceManager.UpdateResource(resource, resourceDocument);
		}

		private class ResourceManagerCollection
		{
			private readonly object _lock = new object();

			private readonly Dictionary<KindDescriptor, ResourceManager> _resourceManagers
				= new Dictionary<KindDescriptor, ResourceManager>();

			public void Add(ResourceManager resourceManager)
			{
				lock (_lock)
				{
					_resourceManagers.Add(resourceManager.Kind.GetKindDescriptor(), resourceManager);
				}
			}

			public void AddRange(IEnumerable<ResourceManager> resourceManagers)
			{
				lock (_lock)
				{
					foreach (var resourceManager in resourceManagers)
					{
						_resourceManagers.Add(resourceManager.Kind.GetKindDescriptor(), resourceManager);
					}
				}
			}

			public bool TryGetResourceManager(KindDescriptor kindDescriptor, [NotNullWhen(true)] out ResourceManager? resourceManager)
			{
				lock (_lock)
				{
					return _resourceManagers.TryGetValue(kindDescriptor, out resourceManager);
				}
			}
		}
	}
}
