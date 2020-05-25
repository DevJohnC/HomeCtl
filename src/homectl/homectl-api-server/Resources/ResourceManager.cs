using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	abstract class ResourceManager
	{
		public abstract Kind Kind { get; }

		public abstract IReadOnlyList<IResource> Resources { get; }

		public abstract Task Save(ResourceState resourceState);

		public abstract Task Load(ResourceOrchestrator orchestrator);
	}

	abstract class ResourceManager<T> : ResourceManager
		where T : class, IResource
	{
		private readonly Dictionary<string, T> _resources = new Dictionary<string, T>();
		protected IResourceDocumentStore<T> DocumentStore { get; }

		public override IReadOnlyList<IResource> Resources => _resources.Values.ToList();

		public override Kind Kind => TypedKind;

		protected abstract Kind<T> TypedKind { get; }

		public ResourceManager(IResourceDocumentStore<T> documentStore)
		{
			DocumentStore = documentStore;
		}

		public override async Task Load(ResourceOrchestrator orchestrator)
		{
			var documents = await DocumentStore.LoadAll();
			foreach (var document in documents)
			{
				await orchestrator.Load(document);
			}
		}

		public override async Task Save(ResourceState resourceState)
		{
			await DocumentStore.Store(resourceState.Identity, resourceState.FullDocument);

			var newResource = CreateFromDocument(resourceState.FullDocument);
			if (newResource == null)
			{
				//  todo: couldn't create resource instance, log error and continue?
				return;
			}

			if (!TryGetResource(resourceState.Identity, out var oldResource))
			{
				_resources.Add(resourceState.Identity, newResource);
				await Created(newResource);
			}
			else
			{
				CopyData(oldResource, newResource);
				await Updated(newResource);
			}
		}

		protected void AddResource(T? resource)
		{
			if (resource == null)
				return;

			_resources.Add(resource.GetIdentity(), resource);
		}

		protected bool TryGetResource(string identifier, [NotNullWhen(true)] out T? resource)
		{
			return _resources.TryGetValue(identifier, out resource);
		}

		protected abstract T? CreateFromDocument(ResourceDocument resourceDocument);

		protected abstract Task Created(T resource);

		protected abstract void CopyData(T target, T source);

		protected abstract Task Updated(T resource);
	}
}
