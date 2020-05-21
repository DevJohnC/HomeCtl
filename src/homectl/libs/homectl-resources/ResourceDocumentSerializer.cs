using HomeCtl.Kinds.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace HomeCtl.Resources
{
	public class ResourceDocumentSerializer
	{
		private static readonly JsonSerializer _jsonSerializer;

		static ResourceDocumentSerializer()
		{
			_jsonSerializer = new JsonSerializer
			{
				Formatting = Formatting.Indented
			};
			_jsonSerializer.Converters.Add(new ResourceDocumentConverter());
			_jsonSerializer.Converters.Add(new KindDescriptorConverter());
			_jsonSerializer.Converters.Add(new ResourceFieldCollectionConverter());
			_jsonSerializer.Converters.Add(new ResourceFieldConverter());
			_jsonSerializer.Converters.Add(new ResourceFieldValueConverter());
			_jsonSerializer.Converters.Add(new ResourceFieldValueCollectionConverter());
		}

		public static string SerializeToJson(ResourceDocument resourceDocument)
		{
			using (var stringWriter = new StringWriter())
			{
				_jsonSerializer.Serialize(stringWriter, resourceDocument, typeof(ResourceDocument));
				return stringWriter.ToString();
			}
		}

		public static ResourceDocument? DeserializeFromJson(string resourceDocumentJson)
		{
			using (var stringReader = new StringReader(resourceDocumentJson))
			using (var jsonReader = new JsonTextReader(stringReader))
			{
				return _jsonSerializer.Deserialize<ResourceDocument>(jsonReader);
			}
		}

		private abstract class CustomConverter<T> : JsonConverter<T>
		{
			protected bool TryReadOverNextPropertyName(JsonReader reader, out string? propertyName)
			{
				propertyName = default;

				if (!reader.Read() || reader.TokenType != JsonToken.PropertyName)
					return false;

				propertyName = reader.Value?.ToString();

				if (!reader.Read())
					return false;

				return true;
			}
		}

		private class ResourceDocumentConverter : CustomConverter<ResourceDocument?>
		{
			public override ResourceDocument? ReadJson(JsonReader reader, Type objectType, ResourceDocument? existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				if (reader.TokenType != JsonToken.StartObject)
					return default;

				KindDescriptor kindDescriptor = default;
				ResourceMetadata? resourceMetadata = default;
				ResourceSpec? resourceSpec = default;
				ResourceState? resourceState = default;

				while (TryReadOverNextPropertyName(reader, out var propertyName))
				{
					switch (propertyName)
					{
						case "kind":
							kindDescriptor = serializer.Deserialize<KindDescriptor>(reader);
							break;
						case "metadata":
							{
								var fieldCollection = serializer.Deserialize<ResourceFieldCollection>(reader);
								if (fieldCollection != null)
									resourceMetadata = new ResourceMetadata(fieldCollection.Fields);
							}
							break;
						case "spec":
							{
								var fieldCollection = serializer.Deserialize<ResourceFieldCollection>(reader);
								if (fieldCollection != null)
									resourceSpec = new ResourceSpec(fieldCollection.Fields);
							}
							break;
						case "state":
							{
								var fieldCollection = serializer.Deserialize<ResourceFieldCollection>(reader);
								if (fieldCollection != null)
									resourceState = new ResourceState(fieldCollection.Fields);
							}
							break;
					}
				}

				if (resourceMetadata == null)
					throw new Exception("Member `metadata` missing from json.");

				return new ResourceDocument(kindDescriptor, resourceMetadata,
					resourceSpec, resourceState);
			}

			public override void WriteJson(JsonWriter writer, ResourceDocument? value, JsonSerializer serializer)
			{
				writer.WriteStartObject();

				if (value != null)
				{
					writer.WritePropertyName("kind");
					serializer.Serialize(writer, value.Kind);

					writer.WritePropertyName("metadata");
					serializer.Serialize(writer, value.Metadata, typeof(ResourceFieldCollection));

					if (value.Spec != null)
					{
						writer.WritePropertyName("spec");
						serializer.Serialize(writer, value.Spec, typeof(ResourceFieldCollection));
					}

					if (value.State != null)
					{
						writer.WritePropertyName("state");
						serializer.Serialize(writer, value.State, typeof(ResourceFieldCollection));
					}
				}

				writer.WriteEndObject();
			}
		}

		private class KindDescriptorConverter : JsonConverter<KindDescriptor?>
		{
			public override KindDescriptor? ReadJson(JsonReader reader, Type objectType, KindDescriptor? existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var jObj = JObject.ReadFrom(reader);

				return new KindDescriptor(
					jObj["group"]?.ToString() ?? throw new Exception("Member `group` missing from json."),
					jObj["apiVersion"]?.ToString() ?? throw new Exception("Member `apiVersion` missing from json."),
					jObj["kindName"]?.ToString() ?? throw new Exception("Member `kindName` missing from json.")
					);
			}

			public override void WriteJson(JsonWriter writer, KindDescriptor? value, JsonSerializer serializer)
			{
				writer.WriteStartObject();

				if (value != null)
				{
					writer.WritePropertyName("kindName");
					writer.WriteValue(value.Value.KindName);

					writer.WritePropertyName("group");
					writer.WriteValue(value.Value.Group);

					writer.WritePropertyName("apiVersion");
					writer.WriteValue(value.Value.ApiVersion);
				}

				writer.WriteEndObject();
			}
		}

		private class ResourceFieldCollectionConverter : JsonConverter<ResourceFieldCollection>
		{
			public override ResourceFieldCollection ReadJson(JsonReader reader, Type objectType, ResourceFieldCollection? existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var fields = new List<ResourceField>();
				while (true)
				{
					var field = serializer.Deserialize<ResourceField>(reader);
					if (field != null)
						fields.Add(field);
					else
						return new ResourceFieldCollection(fields);
				}
			}

			public override void WriteJson(JsonWriter writer, ResourceFieldCollection? value, JsonSerializer serializer)
			{
				writer.WriteStartObject();

				if (value != null)
				{
					foreach (var resourceField in value.Fields)
					{
						serializer.Serialize(writer, resourceField);
					}
				}

				writer.WriteEndObject();
			}
		}

		private class ResourceFieldConverter : CustomConverter<ResourceField?>
		{
			public override ResourceField? ReadJson(JsonReader reader, Type objectType, ResourceField? existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				if (!TryReadOverNextPropertyName(reader, out var fieldName) ||
					fieldName == null)
					return default;

				return new ResourceField(fieldName,
					serializer.Deserialize<ResourceFieldValue>(reader) ?? throw new Exception("Malformed field."));
			}

			public override void WriteJson(JsonWriter writer, ResourceField? value, JsonSerializer serializer)
			{
				if (value == null || value.FieldValue == null)
					return;

				writer.WritePropertyName(value.FieldName);
				serializer.Serialize(writer, value.FieldValue);
			}
		}

		private class ResourceFieldValueCollectionConverter : JsonConverter<ResourceFieldValueCollection>
		{
			public override ResourceFieldValueCollection ReadJson(JsonReader reader, Type objectType, ResourceFieldValueCollection? existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				reader.Read();
				var result = new List<ResourceFieldValue>();
				while (true)
				{
					var field = serializer.Deserialize<ResourceFieldValue>(reader);
					if (field == null)
						return new ResourceFieldValueCollection { Values = result };
					result.Add(field);
					reader.Read();
				}
			}

			public override void WriteJson(JsonWriter writer, ResourceFieldValueCollection? value, JsonSerializer serializer)
			{
				writer.WriteStartArray();

				if (value != null)
				{
					foreach (var fieldValue in value.Values)
					{
						serializer.Serialize(writer, fieldValue, typeof(ResourceFieldValue));
					}
				}

				writer.WriteEndArray();
			}
		}

		private class ResourceFieldValueConverter : JsonConverter<ResourceFieldValue?>
		{
			public override ResourceFieldValue? ReadJson(JsonReader reader, Type objectType, ResourceFieldValue? existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				if (reader.ValueType == typeof(string))
				{
					var result = ResourceFieldValue.String((string?)reader.Value);
					return result;
				}
				else if (reader.TokenType == JsonToken.StartArray)
				{
					return ResourceFieldValue.Array(serializer.Deserialize<ResourceFieldValueCollection>(reader));
				}
				else if (reader.TokenType == JsonToken.StartObject)
				{
					return ResourceFieldValue.Object(serializer.Deserialize<ResourceFieldCollection>(reader));
				}

				return default;
			}

			public override void WriteJson(JsonWriter writer, ResourceFieldValue? value, JsonSerializer serializer)
			{
				if (value == null)
					return;

				switch (value.Type)
				{
					case ResourceFieldValue.ValueType.Array:
						serializer.Serialize(writer, value.GetArray());
						break;
					case ResourceFieldValue.ValueType.Bool:
						writer.WriteValue(value.GetBool());
						break;
					case ResourceFieldValue.ValueType.Int32:
						writer.WriteValue(value.GetInt32());
						break;
					case ResourceFieldValue.ValueType.Int64:
						writer.WriteValue(value.GetInt64());
						break;
					case ResourceFieldValue.ValueType.Object:
						serializer.Serialize(writer, value.GetObject());
						break;
					case ResourceFieldValue.ValueType.String:
						writer.WriteValue(value.GetString());
						break;
				}
			}
		}
	}
}
