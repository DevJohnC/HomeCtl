namespace homectl.Resources
{
	public interface IResource
	{
	}

	public interface IResource<TMetadata, TSpec> : IResource
		where TMetadata : class
		where TSpec : class
	{
		TMetadata Metadata { get; set; }

		TSpec Spec { get; set; }
	}

	public interface IResource<TMetadata, TSpec, TState> : IResource<TMetadata, TSpec>
		where TMetadata : class
		where TSpec : class
		where TState : class
	{
		TState State { get; set; }
	}
}
