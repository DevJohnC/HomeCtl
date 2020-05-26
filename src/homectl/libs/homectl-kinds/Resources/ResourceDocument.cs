namespace HomeCtl.Kinds.Resources
{
	public class ResourceDocument
	{
		public ResourceDocument(ResourceDefinition definition,
			ResourceMetadata? metadata = null,
			ResourceSpec? spec = null,
			ResourceState? state = null,
			KindDescriptor? kind = null)
		{
			Kind = kind;
			Metadata = metadata;
			Definition = definition;
			Spec = spec;
			State = state;
		}

		public KindDescriptor? Kind { get; set; }

		public ResourceMetadata? Metadata { get; set; }

		public ResourceDefinition Definition { get; }

		public ResourceSpec? Spec { get; set; }

		public ResourceState? State { get; set; }

		public ResourceDocument Patch(ResourceDocument patchDocument)
		{
			return new ResourceDocument(
				new ResourceDefinition(ResourceFieldCollection.Patch(Definition.Fields, patchDocument.Definition.Fields)),
				new ResourceMetadata(ResourceFieldCollection.Patch(Metadata?.Fields, patchDocument.Metadata?.Fields)),
				new ResourceSpec(ResourceFieldCollection.Patch(Spec?.Fields, patchDocument.Spec?.Fields)),
				State,
				Kind
				);
		}
	}
}
