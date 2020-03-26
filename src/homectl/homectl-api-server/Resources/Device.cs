namespace homectl.Resources
{
	/// <summary>
	/// Devices are hardware and software with state we want to control.
	/// </summary>
	public class Device : Resource
	{
		public Device(ResourceKind kind, ResourceMetadata metadata, ResourceSpec spec, ResourceState state) : base(kind, metadata, spec, state)
		{
		}
	}
}
