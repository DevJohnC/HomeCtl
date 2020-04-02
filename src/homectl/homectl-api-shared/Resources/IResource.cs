namespace homectl.Resources
{
	public interface IResource
	{
	}

	public interface IResource<TMetadata, TSpec> : IResource
		where TMetadata : class
		where TSpec : class
	{
		new TMetadata Metadata { get; set; }

		new TSpec Spec { get; set; }
	}

	public interface IResource<TMetadata, TSpec, TState> : IResource<TMetadata, TSpec>
		where TMetadata : class
		where TSpec : class
		where TState : class
	{
		new TState State { get; set; }
	}
}
