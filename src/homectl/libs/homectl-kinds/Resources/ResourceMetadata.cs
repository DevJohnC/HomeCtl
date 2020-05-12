using System;
using System.Collections.Generic;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceMetadata : ResourceFieldCollection
	{
		public ResourceMetadata(IList<ResourceField> fields) :
			base(fields)
		{
		}
	}
}
