namespace homectl.Resources
{
	public class Resource
	{
		public static readonly Resource Nothing = new Resource(
			ResourceKind.Nothing, ResourceMetadata.Nothing,
			ResourceSpec.Nothing, ResourceState.Nothing
			);

		public Resource(ResourceKind kind, ResourceMetadata metadata, ResourceSpec spec, ResourceState state)
		{
			Kind = kind;
			Metadata = metadata;
			Spec = spec;
			State = state;
		}

		/// <summary>
		/// Gets the kind of the resource.
		/// </summary>
		public ResourceKind Kind { get; }

		/// <summary>
		/// Gets metadata for the resource.
		/// </summary>
		public ResourceMetadata Metadata { get; }

		/// <summary>
		/// Gets the spec for this resource.
		/// </summary>
		public ResourceSpec Spec { get; }

		/// <summary>
		/// Gets the current state of this resource.
		/// </summary>
		public ResourceState State { get; }
	}
}
