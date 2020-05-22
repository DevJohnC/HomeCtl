using System;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceDocument
	{
		public ResourceDocument(KindDescriptor kind, ResourceMetadata metadata,
			ResourceDefinition definition,
			ResourceSpec? spec = null, ResourceState? state = null)
		{
			Kind = kind;
			Metadata = metadata;
			Definition = definition;
			Spec = spec;
			State = state;
		}

		public KindDescriptor Kind { get; set; }

		public ResourceMetadata Metadata { get; set; }

		public ResourceDefinition Definition { get; }

		public ResourceSpec? Spec { get; set; }

		public ResourceState? State { get; set; }
	}
}
