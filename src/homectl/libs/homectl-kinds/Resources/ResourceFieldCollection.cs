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

		public static IList<ResourceField> Patch(IList<ResourceField>? originalFields, IList<ResourceField>? patchFields)
		{
			var ret = new List<ResourceField>();

			if (originalFields != null)
			{
				ret.AddRange(originalFields);
			}

			if (patchFields == null || patchFields.Count == 0)
			{
				return ret;
			}

			foreach (var patchField in patchFields)
			{
				PatchField(ret, patchField);
			}

			return ret;
		}

		private static void PatchField(IList<ResourceField> fields, ResourceField patchField)
		{
			var existingField = fields.FirstOrDefault(q => q.FieldName == patchField.FieldName);

			if (existingField == null)
			{
				fields.Add(patchField);
				return;
			}

			if (existingField.FieldValue.Type == ResourceFieldValue.ValueType.Object &&
				patchField.FieldValue.Type == ResourceFieldValue.ValueType.Object)
			{
				existingField.FieldValue.SetObject(new ResourceFieldCollection(Patch(
					existingField.FieldValue.GetObject()?.Fields,
					patchField.FieldValue.GetObject()?.Fields
					)));
			}
			else
			{
				existingField.FieldValue = patchField.FieldValue;
			}
		}
	}
}
