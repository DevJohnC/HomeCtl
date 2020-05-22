using System.Collections.Generic;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceDefinition : ResourceFieldCollection
	{
		public ResourceDefinition(IList<ResourceField> fields) : base(fields)
		{
		}
	}
}
