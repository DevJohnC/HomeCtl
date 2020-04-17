namespace HomeCtl.ApiServer.Resources
{
	static class ResourceEvents
	{
		public class ResourceStoredEvent
		{
			public ResourceStoredEvent(Resource resource)
			{
				Resource = resource;
			}

			public Resource Resource { get; }
		}

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
