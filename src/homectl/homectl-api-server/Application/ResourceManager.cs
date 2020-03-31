using homectl.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace homectl.Application
{
	public abstract class ResourceManager
	{
		public ResourceManager(Kind kind)
		{
			Kind = kind ?? throw new ArgumentNullException(nameof(kind));
		}

		public Kind Kind { get; }

		private readonly List<Resource> _resources = new List<Resource>();

		protected void Add(Resource resource)
		{
			_resources.Add(resource);
		}

		public virtual Resource? Create(ResourceMetadata metadata, ResourceSpec spec, Guid? resourceIdentifier)
		{
			return default;
		}

		public IReadOnlyList<Resource> GetAll()
		{
			return _resources;
		}

		public Resource? GetSingle(Guid id)
		{
			return _resources.FirstOrDefault(q => q.Record.Id == id);
		}

		public Resource? GetSingle(ResourceMetadata metadata)
		{
			return _resources.FirstOrDefault(q => q.Metadata.Equals(metadata));
		}

		public virtual Resource? Update(Resource resource, ResourceMetadata metadata, ResourceSpec spec)
		{
			return default;
		}

		public void Remove(Resource resource)
		{
			_resources.Remove(resource);
		}
	}
}