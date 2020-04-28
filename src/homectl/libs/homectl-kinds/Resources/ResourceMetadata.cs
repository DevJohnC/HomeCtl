using System;
using System.Collections.Generic;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceMetadata : ResourceFieldCollection
	{
		public ResourceMetadata(Guid id, string? label, IList<ResourceField> fields) :
			base(fields)
		{
			Id = id;
			Label = label;
		}

		public Guid Id { get; set; }

		public string? Label { get; set; }
	}
}
