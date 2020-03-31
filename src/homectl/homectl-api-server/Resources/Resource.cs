namespace homectl.Resources
{
	public abstract class Resource
	{
		internal Resource(ResourceRecord record, ResourceMetadata metadata, ResourceSpec spec, ResourceState state)
		{
			Metadata = metadata;
			Spec = spec;
			State = state;
			Record = record;
		}

		public Resource(Kind kind, ResourceRecord record, ResourceMetadata metadata, ResourceSpec spec, ResourceState state) :
			this(record, metadata, spec, state)
		{
			Kind = kind;
		}

		/// <summary>
		/// Gets the kind of the resource.
		/// </summary>
		public Kind Kind { get; internal set; }

		/// <summary>
		/// Gets the resource record.
		/// </summary>
		public ResourceRecord Record { get; }

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
