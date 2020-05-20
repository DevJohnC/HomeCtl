using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	abstract class ResourceManager
	{
		public abstract Kind Kind { get; }

		public abstract bool TryGetResource(ResourceDocument resourceDocument, [NotNullWhen(true)] out IResource? resource);

		public abstract Task CreateResource(ResourceDocument resourceDocument);

		public abstract Task UpdateResource(IResource resource, ResourceDocument resourceDocument);

		public abstract Task StoreChanges(IResource resource);

		public abstract Task LoadResources();
	}

	abstract class ResourceManager<T> : ResourceManager
		where T : class, IResource
	{
		private readonly Dictionary<string, T> _resources = new Dictionary<string, T>();
		protected IResourceDocumentStore<T> DocumentStore { get; }

		public override Kind Kind => TypedKind;

		protected abstract Kind<T> TypedKind { get; }

		public ResourceManager(IResourceDocumentStore<T> documentStore)
		{
			DocumentStore = documentStore;
		}

		protected void Add(string key, T resource)
		{
			_resources.Add(key, resource);
		}

		protected abstract bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out string? key);

		protected virtual bool TryConvertToResourceInstance(ResourceDocument resourceDocument, [NotNullWhen(true)]  out T? resourceInstance)
		{
			return TypedKind.TryConvertToResourceInstance(resourceDocument, out resourceInstance);
		}

		protected virtual Task OnResourceCreated(T resource)
			=> Task.CompletedTask;

		protected virtual Task OnResourceUpdated(T newResource, T oldResource)
			=> Task.CompletedTask;

		protected virtual Task OnResourceLoaded(T resource)
			=> Task.CompletedTask;

		public override async Task LoadResources()
		{
			foreach (var document in await DocumentStore.LoadAll())
			{
				if (!TryGetKey(document, out var key) || 
					!TryConvertToResourceInstance(document, out T? resource))
				{
					//  todo: log failure
					continue;
				}

				Add(key, resource);

				await OnResourceLoaded(resource);
			}
		}

		public override async Task CreateResource(ResourceDocument resourceDocument)
		{
			if (!TryGetKey(resourceDocument, out var key) ||
				!TryConvertToResourceInstance(resourceDocument, out T? resource))
			{
				return;
			}

			Add(key, resource);

			await StoreResource(resourceDocument);

			await OnResourceCreated(resource);
		}

		public override async Task UpdateResource(IResource resource, ResourceDocument resourceDocument)
		{
			if (resource.Kind != Kind)
				throw new System.Exception("Mismatched kind.");

			var typedResource = resource as T;
			if (typedResource == null)
				throw new System.Exception("Mismatched kind.");
			if (!TryConvertToResourceInstance(resourceDocument, out T? newVersion))
				throw new System.Exception("Failed to convert document to resource.");

			await StoreResource(resourceDocument);

			await OnResourceUpdated(newVersion, typedResource);
		}

		private Task StoreResource(ResourceDocument resourceDocument)
		{
			if (!TryGetKey(resourceDocument, out var key))
				throw new System.Exception("Failed to store resource: couldn't determine key.");
			return DocumentStore.Store(key, resourceDocument);
		}

		public override Task StoreChanges(IResource resource)
		{
			var typedResource = resource as T;
			if (typedResource == null)
				throw new System.Exception("Invalid resource type.");

			if (!typedResource.Kind.TryConvertToDocument(resource, out var document))
				throw new System.Exception("Failed to convert resource to document.");

			return StoreResource(document);
		}

		protected bool TryGetResource(string key, [NotNullWhen(true)] out T? resource)
		{
			return _resources.TryGetValue(key, out resource);
		}

		public override bool TryGetResource(ResourceDocument resourceDocument, [NotNullWhen(true)] out IResource? resource)
		{
			if (!TryGetKey(resourceDocument, out var key) ||
				!_resources.TryGetValue(key, out var storedResource))
			{
				resource = default;
				return false;
			}

			resource = storedResource;
			return true;
		}
	}
}
