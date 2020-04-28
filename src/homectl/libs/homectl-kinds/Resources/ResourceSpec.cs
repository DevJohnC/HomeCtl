using System.Collections.Generic;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceSpec : ResourceFieldCollection
	{
		public ResourceSpec(IList<ResourceField> fields) : base(fields)
		{
		}

		/*public static ResourceDocumentSpec? FromProto(Servers.ApiServer.ResourceDocumentSpec? protoDocument)
		{
			if (protoDocument == null)
				return null;

			return new ResourceDocumentSpec(
				ConvertFields(protoDocument.Fields)
				);
		}*/
	}
}
