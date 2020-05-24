using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceOrchestrator
	{
		private readonly ResourceManagerCollection _resourceManagers = new ResourceManagerCollection();
		private readonly ResourceManagerAccessor _resourceManagerAccessor;
		private readonly Dictionary<string, ResourceManager> _resourceIdentityIndex = new Dictionary<string, ResourceManager>();

		public ResourceOrchestrator(
			IEnumerable<ResourceManager> coreResourceManagers,
			ResourceManagerAccessor resourceManagerAccessor
			)
		{
			_resourceManagerAccessor = resourceManagerAccessor;
			_resourceManagers.AddRange(coreResourceManagers);
			foreach (var resourceManager in _resourceManagers.GetAll())
				_resourceManagerAccessor.Add(resourceManager);
		}

		private bool TryGetIdentity(ResourceDocument resourceDocument, [NotNullWhen(true)] out string? identity)
		{
			identity = resourceDocument.Definition["identity"]?.GetString();
			return identity != null;
		}

		public void AddResourceManager(ResourceManager resourceManager)
		{
			_resourceManagers.Add(resourceManager);
		}

		public async Task LoadResources()
		{
			foreach (var resourceManager in _resourceManagerAccessor.Managers.ToArray())
			{
				await resourceManager.LoadResources();
			}

			foreach (var resourceManager in _resourceManagerAccessor.Managers)
			{
				foreach (var resource in resourceManager.Resources)
				{
					_resourceIdentityIndex.Add(resource.GetIdentity(), resourceManager);
				}
			}
		}

		private async Task<IResource> CreateResource(string identity, ResourceDocument resourceDocument)
		{
			if (resourceDocument.Kind == null)
			{
				throw new System.Exception("Kind must be specified to create a resource.");
			}

			if (!_resourceManagers.TryGetResourceManager(resourceDocument.Kind.Value, out var resourceManager))
			{
				throw new System.Exception("Unknown kind.");
			}

			var ret = await resourceManager.CreateResource(resourceDocument);

			if (ret == null)
			{
				throw new System.Exception("Unable to create resource.");
			}

			_resourceIdentityIndex.Add(identity, resourceManager);
			return ret;
		}

		public async Task Apply(ResourceDocument resourceDocument)
		{
			if (!TryGetIdentity(resourceDocument, out var identity))
			{
				throw new System.Exception("Identity field is required.");
			}

			var resource = default(IResource);

			if (!_resourceIdentityIndex.TryGetValue(identity, out var resourceManager))
			{
				resource = await CreateResource(identity, resourceDocument);
			}
			else if (!resourceManager.TryGetResource(identity, out resource))
			{
				throw new System.Exception("Update failed, resource missing.");
			}

			//  copy definition fields to `resource`

			if (resourceDocument.Metadata != null)
			{
				//  copy metadata fields to `resource`
			}

			if (resourceDocument.Spec != null)
			{
				//  set new spec fields for `resource`
			}

			//  save
		}

		private class ResourceManagerCollection
		{
			private readonly object _lock = new object();

			private readonly Dictionary<KindDescriptor, ResourceManager> _resourceManagers
				= new Dictionary<KindDescriptor, ResourceManager>();

			public IEnumerable<ResourceManager> GetAll()
			{
				lock (_lock)
				{
					return _resourceManagers.Values.ToList();
				}
			}

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
