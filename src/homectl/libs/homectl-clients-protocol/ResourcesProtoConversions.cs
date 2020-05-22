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
				Definition.ToDefinition(),
				Spec?.ToSpec(),
				State?.ToState()
				);
		}

		public static ResourceDocument FromResourceDocument(Kinds.Resources.ResourceDocument resourceDocument)
		{
			return new ResourceDocument
			{
				Kind = new KindIdentifier
				{
					KindName = resourceDocument.Kind.KindName,
					KindGroup = resourceDocument.Kind.Group,
					KindApiVersion = resourceDocument.Kind.ApiVersion
				},
				Metadata = ResourceDocumentMetadata.FromMetadata(resourceDocument.Metadata),
				Definition = ResourceDocumentDefinition.FromDefinition(resourceDocument.Definition),
				Spec = ResourceDocumentSpec.FromSpec(resourceDocument.Spec),
				State = ResourceDocumentState.FromState(resourceDocument.State)
			};
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

		public static ResourceDocumentMetadata FromMetadata(ResourceMetadata resourceMetadata)
		{
			var ret = new ResourceDocumentMetadata();
			foreach (var field in resourceMetadata.Fields)
				ret.Fields.Add(ResourceDocumentField.FromField(field));
			return ret;
		}
	}

	public partial class ResourceDocumentDefinition
	{
		public ResourceDefinition ToDefinition()
		{
			return new ResourceDefinition(
				ResourceDocumentFieldCollection.ConvertFields(Fields)
				);
		}

		public static ResourceDocumentDefinition FromDefinition(ResourceDefinition resourceDefinition)
		{
			var ret = new ResourceDocumentDefinition();
			foreach (var field in resourceDefinition.Fields)
				ret.Fields.Add(ResourceDocumentField.FromField(field));
			return ret;
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

		public static ResourceDocumentSpec? FromSpec(ResourceSpec? resourceSpec)
		{
			if (resourceSpec == null)
				return default;

			var ret = new ResourceDocumentSpec();
			foreach (var field in resourceSpec.Fields)
				ret.Fields.Add(ResourceDocumentField.FromField(field));
			return ret;
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

		public static ResourceDocumentState? FromState(ResourceState? resourceState)
		{
			if (resourceState == null)
				return default;

			var ret = new ResourceDocumentState();
			foreach (var field in resourceState.Fields)
				ret.Fields.Add(ResourceDocumentField.FromField(field));
			return ret;
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

		public static ResourceDocumentField FromField(ResourceField resourceField)
		{
			return new ResourceDocumentField
			{
				FieldName = resourceField.FieldName,
				FieldValue = ResourceDocumentValue.FromFieldValue(resourceField.FieldValue)
			};
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

		public static ResourceDocumentFieldCollection FromFieldCollection(IEnumerable<ResourceField> resourceFields)
		{
			var ret = new ResourceDocumentFieldCollection();
			foreach (var field in resourceFields)
				ret.Fields.Add(ResourceDocumentField.FromField(field));
			return ret;
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

		public static ResourceDocumentValueCollection FromValueCollection(IEnumerable<ResourceFieldValue> values)
		{
			var ret = new ResourceDocumentValueCollection();
			foreach (var value in values)
				ret.Values.Add(ResourceDocumentValue.FromFieldValue(value));
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

		public static ResourceDocumentValue FromFieldValue(ResourceFieldValue fieldValue)
		{
			switch (fieldValue.Type)
			{
				case ResourceFieldValue.ValueType.Bool:
					return new ResourceDocumentValue
					{
						BoolValue = fieldValue.GetBool()
					};
				case ResourceFieldValue.ValueType.Int32:
					return new ResourceDocumentValue
					{
						Int32Value = fieldValue.GetInt32()
					};
				case ResourceFieldValue.ValueType.Int64:
					return new ResourceDocumentValue
					{
						Int64Value = fieldValue.GetInt64()
					};
				case ResourceFieldValue.ValueType.String:
					return new ResourceDocumentValue
					{
						StringValue = fieldValue.GetString()
					};
				case ResourceFieldValue.ValueType.Object:
					return new ResourceDocumentValue
					{
						ObjectValue = ResourceDocumentFieldCollection.FromFieldCollection(
							fieldValue.GetObject()?.Fields ?? new ResourceField[0])
					};
				case ResourceFieldValue.ValueType.Array:
					return new ResourceDocumentValue
					{
						ArrayOfValues = ResourceDocumentValueCollection.FromValueCollection(
							fieldValue.GetArray().Values)
					};
			}
			throw new Exception("Invalid value type.");
		}
	}
}
