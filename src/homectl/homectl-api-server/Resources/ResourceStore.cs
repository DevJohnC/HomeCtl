using System;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceStore
	{
		private readonly Events.EventBus _eventBus;

		public ResourceStore(Events.EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		public bool TryGetResource(Guid id, KindDescriptor kind, out Resource resource)
		{
			resource = default;
			return false;
		}

		public bool TryStoreResource(Resource resource)
		{
			return TryStoreResource(resource, out var _);
		}

		public bool TryStoreResource(Resource resource, out bool updatedExistingResource)
		{
			//  validate Kind
			//  validate data against kind schema
			//  store
			//  publish event
			_eventBus.Publish(new ResourceEvents.ResourceStoredEvent(resource));
			updatedExistingResource = false;
			return true;
		}
	}
}
