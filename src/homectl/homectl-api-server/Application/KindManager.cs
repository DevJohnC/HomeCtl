using homectl_api_server.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace homectl_api_server.Application
{
	public class KindManager
	{
		public static readonly KindManager Nothing = new KindManager(ResourceKind.Nothing);

		public KindManager(ResourceKind kind)
		{
			Kind = kind ?? throw new ArgumentNullException(nameof(kind));
		}

		public ResourceKind Kind { get; }

		private readonly List<Resource> _resources = new List<Resource>();

		protected void Add(Resource resource)
		{
			_resources.Add(resource);
		}

		public virtual Resource Create(ResourceMetadata metadata, ResourceSpec spec)
		{
			return Resource.Nothing;
		}

		public IReadOnlyList<Resource> GetAll()
		{
			return _resources;
		}

		public Resource GetSingle(Guid id)
		{
			return _resources.FirstOrDefault(q => q.Metadata.Id == id) ??
				Resource.Nothing;
		}

		public void UpdateSpec(Resource resource, ResourceMetadata metadata, ResourceSpec spec)
		{

		}

		public void Remove(Resource resource)
		{
			_resources.Remove(resource);
		}
	}
}