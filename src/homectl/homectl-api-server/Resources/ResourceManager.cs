using HomeCtl.Kinds.Resources;
using System;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceManager
	{
		private readonly Events.EventBus _eventBus;

		public ResourceManager(Events.EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		public void ApplyChanges(ResourceDocumentStoreResult resourceDocumentStoreResult)
		{

		}
	}
}
