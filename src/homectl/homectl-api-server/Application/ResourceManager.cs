using homectl.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace homectl.Application
{
	public abstract class ResourceManager
	{
		public ResourceManager(KindDescriptor kind)
		{
			Kind = kind ?? throw new ArgumentNullException(nameof(kind));
		}

		public KindDescriptor Kind { get; }

		private readonly Dictionary<Guid, ResourceRecordPair> _resources = new Dictionary<Guid, ResourceRecordPair>();

		protected void Add(ResourceRecord record, IResource resource)
		{
			_resources.Add(
				record.Id,
				new ResourceRecordPair(record, resource)
				);
		}

		public virtual bool TryCreate(JObject metadata, JObject spec, Guid? resourceIdentifier,
			out ResourceRecordPair resourceRecordPair)
		{
			resourceRecordPair = default;
			return false;
		}

		public IReadOnlyCollection<ResourceRecordPair> GetAll()
		{
			return _resources.Values;
		}

		public bool TryGetSingle(Guid id, out ResourceRecordPair resourceRecordPair)
		{
			return _resources.TryGetValue(id, out resourceRecordPair);
		}

		public virtual bool TryUpdate(ResourceRecordPair resource, JObject metadata, JObject spec, out ResourceRecordPair resourceRecordPair)
		{
			resourceRecordPair = default;
			return false;
		}

		public void Remove(ResourceRecord record)
		{
			_resources.Remove(record.Id);
		}

		public struct ResourceRecordPair
		{
			public ResourceRecordPair(ResourceRecord record, IResource resource)
			{
				Record = record;
				Resource = resource;
			}

			public ResourceRecord Record { get; }

			public IResource Resource { get; }
		}
	}
}