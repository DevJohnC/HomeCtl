using System.Collections.Generic;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceState : ResourceFieldCollection
	{
		public ResourceState(IList<ResourceField> fields) : base(fields)
		{
		}

		/*public static ResourceDocumentState? FromProto(Servers.ApiServer.ResourceDocumentState? protoDocument)
		{
			if (protoDocument == null)
				return null;

			return new ResourceDocumentState(
				ConvertFields(protoDocument.Fields)
				);
		}*/
	}
}
