using System.Collections.Generic;
using System.Linq;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceFieldCollection
	{
		public ResourceFieldCollection(IList<ResourceField> fields)
		{
			Fields = fields;
			_indexedFields = fields.ToDictionary(q => q.FieldName);
		}

		public IList<ResourceField> Fields { get; private set; }

		private readonly Dictionary<string, ResourceField> _indexedFields;

		public ResourceFieldValue? this[string fieldName]
		{
			get
			{
				if (!_indexedFields.TryGetValue(fieldName, out var field))
					return default;
				return field.FieldValue;
			}
		}

		/*public static ResourceDocumentFieldCollection? FromProto(Servers.ApiServer.ResourceDocumentFieldCollection? protoDocument)
		{
			if (protoDocument == null)
				return null;

			return new ResourceDocumentFieldCollection(
				ConvertFields(protoDocument.Fields)
				);
		}

		protected static IList<ResourceDocumentField> ConvertFields(IEnumerable<Servers.ApiServer.ResourceDocumentField> fields)
		{
			return fields
				.Select(q => ResourceDocumentField.FromProto(q))
				.ToList();
		}*/
	}
}
