using HomeCtl.ApiServer.Kinds;
using HomeCtl.Events;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using HomeCtl.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ResourceDocument = HomeCtl.Kinds.Resources.ResourceDocument;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceOrchestrator
	{
		private readonly ResourceManagerCollection _resourceManagers = new ResourceManagerCollection();
		private readonly ResourceManagerAccessor _resourceManagerAccessor;
		private readonly Dictionary<string, ResourceState> _resourceIdentityIndex = new Dictionary<string, ResourceState>();

		public ResourceOrchestrator(
			IEnumerable<ResourceManager> coreResourceManagers,
			ResourceManagerAccessor resourceManagerAccessor,
			EventBus eventBus
			)
		{
			_resourceManagerAccessor = resourceManagerAccessor;
			_resourceManagers.AddRange(coreResourceManagers);
			foreach (var resourceManager in _resourceManagers.GetAll())
				_resourceManagerAccessor.Add(resourceManager);

			eventBus.Subscribe<ResourceManagerAccessorEvents.ResourceManagerAdded>(
				args => EnsureResourceManagerIsPresent(args.ResourceManager));
		}

		private bool TryGetIdentity(ResourceDocument resourceDocument, [NotNullWhen(true)] out string? identity)
		{
			identity = resourceDocument.Definition["identity"]?.GetString();
			return identity != null;
		}

		public async Task LoadResources()
		{
			foreach (var resourceManager in _resourceManagers.GetAll())
			{
				await resourceManager.Load(this);
			}
		}

		private void EnsureResourceManagerIsPresent(ResourceManager resourceManager)
		{
			if (!_resourceManagers.TryGetResourceManager(resourceManager.Kind.GetKindDescriptor(), out var _))
			{
				_resourceManagers.Add(resourceManager);
			}
		}

		private bool TryGetResourceState(string identity, out ResourceState fullResourceState)
		{
			return _resourceIdentityIndex.TryGetValue(identity, out fullResourceState);
		}

		private void CommitResourceState(ResourceState resourceState)
		{
			_resourceIdentityIndex[resourceState.Identity] = resourceState;
		}

		private bool TryGetKindManager(ResourceDocument partialResourceDocument, [NotNullWhen(true)] out ResourceManager? kindManager)
		{
			if (partialResourceDocument.Kind == null)
			{
				kindManager = default;
				return false;
			}

			return _resourceManagers.TryGetResourceManager(partialResourceDocument.Kind.Value, out kindManager);
		}

		private ResourceState CreateDefaultState(string identity, ResourceDocument partialResourceState)
		{
			if (!TryGetKindManager(partialResourceState, out var manager))
				throw new System.Exception("Valid kind required.");

			//  todo: create default document from kind schema
			var stateDoc = new ResourceDocument(
				new ResourceDefinition(new List<ResourceField>
				{
					new ResourceField("identity", ResourceFieldValue.String(identity))
				}));

			return new ResourceState(manager, identity, stateDoc);
		}

		private ResourceState ApplyFields(ResourceDocument partialResourceState, ResourceState existingState)
		{
			return existingState;
		}

		public bool Validate(ResourceState resourceState)
		{
			return true;
		}

		public Task Load(ResourceDocument fullResourceState)
		{
			if (!TryGetIdentity(fullResourceState, out var identity))
			{
				throw new System.Exception("Identity field is required.");
			}

			if (!TryGetKindManager(fullResourceState, out var manager))
			{
				throw new System.Exception("Valid kind required.");
			}

			var newState = new ResourceState(
				manager, identity, fullResourceState
				);

			if (!Validate(newState))
			{
				throw new System.Exception("Invalid resource parameters specified.");
			}

			CommitResourceState(newState);

			return newState.Manager.Save(newState);
		}

		public Task Apply(ResourceDocument partialResourceState)
		{
			if (!TryGetIdentity(partialResourceState, out var identity))
			{
				throw new System.Exception("Identity field is required.");
			}

			if (!TryGetResourceState(identity, out var existingState))
			{
				existingState = CreateDefaultState(identity, partialResourceState);
			}

			var newState = ApplyFields(partialResourceState, existingState);

			if (!Validate(newState))
			{
				throw new System.Exception("Invalid resource parameters specified.");
			}

			CommitResourceState(newState);

			return newState.Manager.Save(newState);
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
