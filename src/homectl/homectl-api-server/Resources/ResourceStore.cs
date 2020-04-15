using HomeCtl.Kinds;
using Newtonsoft.Json.Linq;
using System;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceStore
	{
		private readonly EventBus.EventBus _eventBus;

		public ResourceStore(EventBus.EventBus eventBus)
		{
			_eventBus = eventBus;
		}

		public bool TryGetResource(ResourceRecord record, KindDescriptor kind, out Resource resource)
		{
			resource = default;
			return false;
		}

		public bool TryStoreResource(Resource resource, out Resource storedResource)
		{
			//  validate Kind
			//  store
			//  publish
			_eventBus.Publish(new ResourceEvents.ResourceCreatedEvent(resource));
			//  assign the version of the resource that was actually stored
			//  incase any data was removed from the metadata/spec data
			storedResource = resource;
			return true;
		}
	}
}
