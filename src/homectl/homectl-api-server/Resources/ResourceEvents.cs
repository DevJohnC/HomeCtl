namespace HomeCtl.ApiServer.Resources
{
	static class ResourceEvents
	{
		public class ResourceCreatedEvent
		{
			public ResourceCreatedEvent(Resource resource)
			{
				Resource = resource;
			}

			public Resource Resource { get; }
		}
	}
}
