using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceDocument
	{
		public ResourceDocument(ResourceDocumentMetadata metadata, ResourceDocumentSpec? spec, ResourceDocumentState? state)
		{
			Metadata = metadata;
			Spec = spec;
			State = state;
		}

		public ResourceDocumentMetadata Metadata { get; set; }

		public ResourceDocumentSpec? Spec { get; set; }

		public ResourceDocumentState? State { get; set; }

		public static ResourceDocument FromProto(Servers.ApiServer.ResourceDocument protoDocument)
		{
			if (protoDocument.Metadata == null)
				throw new Exception("Metadata required.");

			return new ResourceDocument(
				ResourceDocumentMetadata.FromProto(protoDocument.Metadata),
				ResourceDocumentSpec.FromProto(protoDocument.Spec),
				ResourceDocumentState.FromProto(protoDocument.State)
				);
		}
	}

	class ResourceDocumentField
	{
		public ResourceDocumentField(string fieldName, ResourceDocumentValue fieldValue)
		{
			FieldName = fieldName;
			FieldValue = fieldValue;
		}

		public string FieldName { get; set; }

		public ResourceDocumentValue FieldValue { get; set; }

		public static ResourceDocumentField FromProto(Servers.ApiServer.ResourceDocumentField protoDocument)
		{
			return new ResourceDocumentField(
				protoDocument.FieldName,
				ResourceDocumentValue.FromProto(protoDocument.FieldValue)
				);
		}
	}

	class ResourceDocumentValue
	{
		private object? _value;
		private ValueType _type;

		public ValueType Type => _type;

		public object? Value => _value;

		public void SetBool(bool value)
		{
			_value = value;
			_type = ValueType.Bool;
		}

		public void SetInt32(int value)
		{
			_value = value;
			_type = ValueType.Int32;
		}

		public void SetInt64(long value)
		{
			_value = value;
			_type = ValueType.Int64;
		}

		public void SetString(string value)
		{
			_value = value;
			_type = ValueType.String;
		}

		public void SetObject(ResourceDocumentFieldCollection? value)
		{
			_value = value;
			_type = ValueType.Object;
		}

		public static ResourceDocumentValue FromProto(Servers.ApiServer.ResourceDocumentValue protoDocument)
		{
			switch (protoDocument.ValueTypeCase)
			{
				case Servers.ApiServer.ResourceDocumentValue.ValueTypeOneofCase.BoolValue:
					{
						var val = new ResourceDocumentValue();
						val.SetBool(protoDocument.BoolValue);
						return val;
					}
				case Servers.ApiServer.ResourceDocumentValue.ValueTypeOneofCase.Int32Value:
					{
						var val = new ResourceDocumentValue();
						val.SetInt32(protoDocument.Int32Value);
						return val;
					}
				case Servers.ApiServer.ResourceDocumentValue.ValueTypeOneofCase.Int64Value:
					{
						var val = new ResourceDocumentValue();
						val.SetInt64(protoDocument.Int64Value);
						return val;
					}
				case Servers.ApiServer.ResourceDocumentValue.ValueTypeOneofCase.StringValue:
					{
						var val = new ResourceDocumentValue();
						val.SetString(protoDocument.StringValue);
						return val;
					}
				case Servers.ApiServer.ResourceDocumentValue.ValueTypeOneofCase.ObjectValue:
					{
						var val = new ResourceDocumentValue();
						val.SetObject(ResourceDocumentFieldCollection.FromProto(protoDocument.ObjectValue));
						return val;
					}
			}
			throw new Exception("Invalid value type.");
		}

		public enum ValueType
		{
			Bool,
			Int32,
			Int64,
			String,
			Object
		}
	}

	class ResourceDocumentFieldCollection
	{
		public ResourceDocumentFieldCollection(IList<ResourceDocumentField> fields)
		{
			Fields = fields;
			_indexedFields = fields.ToDictionary(q => q.FieldName);
		}

		public IList<ResourceDocumentField> Fields { get; private set; }

		private readonly Dictionary<string, ResourceDocumentField> _indexedFields;

		public ResourceDocumentValue? this[string fieldName]
		{
			get
			{
				if (!_indexedFields.TryGetValue(fieldName, out var field))
					return default;
				return field.FieldValue;
			}
		}

		public static ResourceDocumentFieldCollection? FromProto(Servers.ApiServer.ResourceDocumentFieldCollection? protoDocument)
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
		}
	}

	class ResourceDocumentMetadata : ResourceDocumentFieldCollection
	{
		public ResourceDocumentMetadata(Guid id, string? label, IList<ResourceDocumentField> fields) :
			base(fields)
		{
			Id = id;
			Label = label;
		}

		public Guid Id { get; set; }

		public string? Label { get; set; }

		public static ResourceDocumentMetadata FromProto(Servers.ApiServer.ResourceDocumentMetadata protoDocument)
		{
			if (!Guid.TryParse(protoDocument.Id, out var parsedId))
				throw new Exception("Id format invalid.");

			return new ResourceDocumentMetadata(
				parsedId,
				protoDocument.Label,
				ConvertFields(protoDocument.Fields)
				);
		}
	}

	class ResourceDocumentSpec : ResourceDocumentFieldCollection
	{
		public ResourceDocumentSpec(IList<ResourceDocumentField> fields) : base(fields)
		{
		}

		public static ResourceDocumentSpec? FromProto(Servers.ApiServer.ResourceDocumentSpec? protoDocument)
		{
			if (protoDocument == null)
				return null;

			return new ResourceDocumentSpec(
				ConvertFields(protoDocument.Fields)
				);
		}
	}

	class ResourceDocumentState : ResourceDocumentFieldCollection
	{
		public ResourceDocumentState(IList<ResourceDocumentField> fields) : base(fields)
		{
		}

		public static ResourceDocumentState? FromProto(Servers.ApiServer.ResourceDocumentState? protoDocument)
		{
			if (protoDocument == null)
				return null;

			return new ResourceDocumentState(
				ConvertFields(protoDocument.Fields)
				);
		}
	}
}
