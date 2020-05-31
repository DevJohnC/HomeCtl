using HomeCtl.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceManagerContainer
	{
		private readonly object _lock = new object();
		private readonly List<ResourceManager> _resourceManagers = new List<ResourceManager>();
		private readonly EventBus _eventBus;

		public ResourceManagerContainer(EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		public IReadOnlyList<ResourceManager> Managers => _resourceManagers;

		public void Add(ResourceManager resourceManager)
		{
			lock (_lock)
				_resourceManagers.Add(resourceManager);
			_eventBus.Publish(new ResourceManagerContainerEvents.ResourceManagerAdded(resourceManager));
		}

		public bool TryFind(Func<ResourceManager, bool> predicate, [NotNullWhen(true)] out ResourceManager? resourceManager)
		{
			lock (_lock)
			{
				resourceManager = _resourceManagers.FirstOrDefault(predicate);
				return resourceManager != null;
			}
		}
	}

	static class ResourceManagerContainerEvents
	{
		public class ResourceManagerAdded
		{
			public ResourceManagerAdded(ResourceManager resourceManager)
			{
				ResourceManager = resourceManager;
			}

			public ResourceManager ResourceManager { get; }
		}
	}
}
