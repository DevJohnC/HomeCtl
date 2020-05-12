using HomeCtl.Kinds.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCtl.Services
{
	public partial class ResourceDocument
	{
		public Kinds.Resources.ResourceDocument ToResourceDocument()
		{
			if (Metadata == null)
				throw new Exception("Metadata required.");

			return new Kinds.Resources.ResourceDocument(
				new KindDescriptor(
					Kind.KindGroup, Kind.KindApiVersion,
					Kind.KindName),
				Metadata.ToMetadata(),
				Spec?.ToSpec(),
				State?.ToState()
				);
		}
	}

	public partial class ResourceDocumentMetadata
	{
		public ResourceMetadata ToMetadata()
		{
			return new ResourceMetadata(
				ResourceDocumentFieldCollection.ConvertFields(Fields)
				);
		}
	}

	public partial class ResourceDocumentSpec
	{
		public ResourceSpec ToSpec()
		{
			return new ResourceSpec(
				ResourceDocumentFieldCollection.ConvertFields(Fields)
				);
		}
	}

	public partial class ResourceDocumentState
	{
		public ResourceState ToState()
		{
			return new ResourceState(
				ResourceDocumentFieldCollection.ConvertFields(Fields)
				);
		}
	}

	public partial class ResourceDocumentField
	{
		public ResourceField ToField()
		{
			return new ResourceField(
				FieldName,
				FieldValue.ToFieldValue()
				);
		}
	}

	public partial class ResourceDocumentFieldCollection
	{
		public ResourceFieldCollection ToFieldCollection()
		{
			return new ResourceFieldCollection(
				ConvertFields(Fields)
				);
		}

		internal static IList<ResourceField> ConvertFields(IEnumerable<ResourceDocumentField> fields)
		{
			return fields
				.Select(q => q.ToField())
				.ToList();
		}
	}

	public partial class ResourceDocumentValueCollection
	{
		public ResourceFieldValueCollection ToValueCollection()
		{
			var ret = new ResourceFieldValueCollection();
			foreach (var value in Values)
			{
				ret.Values.Add(value.ToFieldValue());
			}
			return ret;
		}
	}

	public partial class ResourceDocumentValue
	{
		public ResourceFieldValue ToFieldValue()
		{
			switch (ValueTypeCase)
			{
				case ValueTypeOneofCase.BoolValue:
					return ResourceFieldValue.Bool(BoolValue);
				case ValueTypeOneofCase.Int32Value:
					return ResourceFieldValue.Int32(Int32Value);
				case ValueTypeOneofCase.Int64Value:
					return ResourceFieldValue.Int64(Int64Value);
				case ValueTypeOneofCase.StringValue:
					return ResourceFieldValue.String(StringValue);
				case ValueTypeOneofCase.ObjectValue:
					return ResourceFieldValue.Object(ObjectValue.ToFieldCollection());
				case ValueTypeOneofCase.ArrayOfValues:
					return ResourceFieldValue.Array(ArrayOfValues.ToValueCollection());
			}
			throw new Exception("Invalid value type.");
		}
	}
}
