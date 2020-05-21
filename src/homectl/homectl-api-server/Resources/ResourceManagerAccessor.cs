using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceManagerAccessor
	{
		private readonly object _lock = new object();
		private readonly List<ResourceManager> _resourceManagers = new List<ResourceManager>();

		public ResourceOrchestrator Orchestrator { get; set; }

		public void Add(ResourceManager resourceManager)
		{
			lock (_lock)
				_resourceManagers.Add(resourceManager);
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
}
