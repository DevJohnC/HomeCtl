using System.Collections.Generic;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceSpec : ResourceFieldCollection
	{
		public ResourceSpec(IList<ResourceField> fields) : base(fields)
		{
		}
	}
}
