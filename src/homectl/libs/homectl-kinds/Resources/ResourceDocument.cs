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

		/*public static ResourceDocument FromProto(Servers.ApiServer.ResourceDocument protoDocument)
		{
			if (protoDocument.Metadata == null)
				throw new Exception("Metadata required.");

			return new ResourceDocument(
				new KindDescriptor(
					protoDocument.Kind.KindGroup, protoDocument.Kind.KindApiVersion,
					protoDocument.Kind.KindName),
				ResourceMetadata.FromProto(protoDocument.Metadata),
				ResourceSpec.FromProto(protoDocument.Spec),
				ResourceState.FromProto(protoDocument.State)
				);
		}*/
	}
}
