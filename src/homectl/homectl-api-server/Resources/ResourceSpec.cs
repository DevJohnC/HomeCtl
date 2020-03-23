namespace homectl_api_server.Resources
{
	public class ResourceSpec : ResourceDocument<ResourceSpec>
	{
		public static readonly ResourceSpec Nothing = new ResourceSpec();

		public bool Validate(ResourceSpec desiredStateSpec)
		{
			return false;
		}
	}
}
