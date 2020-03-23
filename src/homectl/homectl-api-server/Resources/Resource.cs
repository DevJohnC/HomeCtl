namespace homectl_api_server.Resources
{
	public class Resource
	{
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
