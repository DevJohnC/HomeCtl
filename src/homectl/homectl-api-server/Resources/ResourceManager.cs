using System;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceManager
	{
		public bool TryGetResource(string group, string apiVersion, string kindName, Guid id, out Resource resource)
		{
			resource = default;
			return false;
		}
	}
}
