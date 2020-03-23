namespace homectl_api_server.Resources
{
	public class Resource
	{
		public static readonly Resource Nothing = new Resource();

		/// <summary>
		/// Gets the kind of the resource.
		/// </summary>
		public ResourceKind Kind { get; } = ResourceKind.Nothing;

		/// <summary>
		/// Gets metadata for the resource.
		/// </summary>
		public ResourceMetadata Metadata { get; } = ResourceMetadata.Nothing;

		/// <summary>
		/// Gets the spec for this resource.
		/// </summary>
		public ResourceSpec Spec { get; } = ResourceSpec.Nothing;

		/// <summary>
		/// Gets the current state of this resource.
		/// </summary>
		public ResourceState State { get; } = ResourceState.Nothing;
	}
}
