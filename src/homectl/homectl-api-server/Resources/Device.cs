namespace homectl.Resources
{
	/// <summary>
	/// Devices are hardware and software with state we want to control.
	/// </summary>
	public class Device : Resource
	{
		public Device(Kind kind, ResourceRecord record, ResourceMetadata metadata, ResourceSpec spec, ResourceState state) : base(kind, record, metadata, spec, state)
		{
		}
	}
}
