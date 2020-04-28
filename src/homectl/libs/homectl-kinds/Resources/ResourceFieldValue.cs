using System.Collections.Generic;
using System.Linq;

namespace HomeCtl.Kinds.Resources
{
	public class ResourceFieldValue
	{
		private ResourceFieldValue() { }

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

		public void SetString(string? value)
		{
			_value = value;
			_type = ValueType.String;
		}

		public void SetObject(ResourceFieldCollection? value)
		{
			_value = value;
			_type = ValueType.Object;
		}

		public void SetArray(ResourceFieldValueCollection valueCollection)
		{
			_value = valueCollection;
			_type = ValueType.Array;
		}

		public static ResourceFieldValue Bool(bool value)
		{
			var ret = new ResourceFieldValue();
			ret.SetBool(value);
			return ret;
		}

		public static ResourceFieldValue Int32(int value)
		{
			var ret = new ResourceFieldValue();
			ret.SetInt32(value);
			return ret;
		}

		public static ResourceFieldValue Int64(long value)
		{
			var ret = new ResourceFieldValue();
			ret.SetInt64(value);
			return ret;
		}

		public static ResourceFieldValue String(string? value)
		{
			var ret = new ResourceFieldValue();
			ret.SetString(value);
			return ret;
		}

		public static ResourceFieldValue Object(IDictionary<string, ResourceFieldValue> valueDict)
		{
			return Object(
				valueDict.Select(q => new ResourceField(q.Key, q.Value)).ToList()
				);
		}

		public static ResourceFieldValue Object(IList<ResourceField> valueList)
		{
			return Object(new ResourceFieldCollection(
				valueList
				));
		}

		public static ResourceFieldValue Object(ResourceFieldCollection? value)
		{
			var ret = new ResourceFieldValue();
			ret.SetObject(value);
			return ret;
		}

		public static ResourceFieldValue Array(IEnumerable<ResourceFieldValue> values)
		{
			return Array(new ResourceFieldValueCollection
			{
				Values = values.ToArray()
			});
		}

		public static ResourceFieldValue Array(ResourceFieldValueCollection valueCollection)
		{
			var ret = new ResourceFieldValue();
			ret.SetArray(valueCollection);
			return ret;
		}

		public enum ValueType
		{
			Bool,
			Int32,
			Int64,
			String,
			Object,
			Array
		}
	}
}
