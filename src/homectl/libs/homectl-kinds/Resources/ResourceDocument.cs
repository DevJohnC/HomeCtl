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
			var definition = new ResourceDefinition(ResourceFieldCollection.Patch(Definition.Fields, patchDocument.Definition.Fields));
			var metadata = (Metadata != null) ?
				new ResourceMetadata(ResourceFieldCollection.Patch(Metadata?.Fields, patchDocument.Metadata?.Fields)) : null;
			var spec = (Spec != null) ?
				new ResourceSpec(ResourceFieldCollection.Patch(Spec?.Fields, patchDocument.Spec?.Fields)) : null;
			var state = (State != null) ?
				new ResourceState(ResourceFieldCollection.Patch(State?.Fields, patchDocument.State?.Fields)) : null;

			return new ResourceDocument(
				definition, metadata,
				spec, state,
				Kind
				);
		}
	}
}
