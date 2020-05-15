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
	}

	abstract class ResourceManager<TKey, T> : ResourceManager
		where T : class, IResource
	{
		private readonly Dictionary<TKey, T> _resources = new Dictionary<TKey, T>();

		public override Kind Kind => TypedKind;

		protected abstract Kind<T> TypedKind { get; }

		protected abstract bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out TKey key);

		public override Task CreateResource(ResourceDocument resourceDocument)
		{
			if (!TryGetKey(resourceDocument, out var key) ||
				!TypedKind.TryConvertToResourceInstance(resourceDocument, out T? resource))
			{
				return Task.CompletedTask;
			}

			_resources.Add(key, resource);

			return Task.CompletedTask;
		}

		public override Task UpdateResource(IResource resource, ResourceDocument resourceDocument)
		{
			if (resource.Kind != Kind)
				throw new System.Exception("Mismatched kind.");

			if (!TryGetKey(resourceDocument, out var key))
				return Task.CompletedTask;

			var typedResource = resource as T;
			if (typedResource == null)
				throw new System.Exception("Mismatched kind.");

			_resources[key] = typedResource;

			return Task.CompletedTask;
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
