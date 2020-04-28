namespace HomeCtl.Kinds.Resources
{
	public class ResourceField
	{
		public ResourceField(string fieldName, ResourceFieldValue fieldValue)
		{
			FieldName = fieldName;
			FieldValue = fieldValue;
		}

		public string FieldName { get; set; }

		public ResourceFieldValue FieldValue { get; set; }

		/*public static ResourceDocumentField FromProto(Servers.ApiServer.ResourceDocumentField protoDocument)
		{
			return new ResourceDocumentField(
				protoDocument.FieldName,
				ResourceDocumentValue.FromProto(protoDocument.FieldValue)
				);
		}*/
	}
}
