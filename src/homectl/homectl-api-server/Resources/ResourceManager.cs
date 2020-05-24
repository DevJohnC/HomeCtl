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

		public abstract Task<IResource?> CreateResource(ResourceDocument resourceDocument);

		public abstract bool TryGetResource(string identity, [NotNullWhen(true)] out IResource? resource);

		public abstract Task LoadResources();
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

		public override async Task<IResource?> CreateResource(ResourceDocument resourceDocument)
		{
			var resource = CreateFromDocument(resourceDocument);
			AddResource(resource);
			if (resource != null)
				await Created(resource);
			return resource;
		}

		public override async Task LoadResources()
		{
			var documents = await DocumentStore.LoadAll();
			foreach (var document in documents)
			{
				var resource = CreateFromDocument(document);
				AddResource(resource);
				if (resource == null)
				{
					//  todo: log failure to load
				}
				else
				{
					await Loaded(resource);
				}
			}
		}

		protected void AddResource(T? resource)
		{
			if (resource == null)
				return;

			_resources.Add(resource.GetIdentity(), resource);
		}

		public override bool TryGetResource(string identity, [NotNullWhen(true)] out IResource? resource)
		{
			if (!_resources.TryGetValue(identity, out var typedResource))
			{
				resource = default;
				return false;
			}

			resource = typedResource;
			return true;
		}

		protected abstract T? CreateFromDocument(ResourceDocument resourceDocument);

		protected abstract Task Created(T resource);

		protected abstract Task Loaded(T resource);
	}
}
