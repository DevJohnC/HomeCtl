using System;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceDocument
	{
		public ResourceDocument(KindDescriptor kind, ResourceMetadata metadata,
			ResourceSpec? spec, ResourceState? state)
		{
			Kind = kind;
			Metadata = metadata;
			Spec = spec;
			State = state;
		}

		public KindDescriptor Kind { get; set; }

		public ResourceMetadata Metadata { get; set; }

		public ResourceSpec? Spec { get; set; }

		public ResourceState? State { get; set; }
	}
}
