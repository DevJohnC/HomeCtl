using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace homectl.JsonConverters
{
	public class OpenApiSchemaConverter : JsonConverter<OpenApiSchema>
	{
		public override OpenApiSchema ReadJson(JsonReader reader, Type objectType, OpenApiSchema existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, OpenApiSchema value, JsonSerializer serializer)
		{
			using (var textWriter = new System.IO.StringWriter())
			{
				var openApiWriter = new OpenApiJsonWriter(textWriter);
				value.SerializeAsV3WithoutReference(openApiWriter);
				serializer.Serialize(writer, JToken.Parse(textWriter.ToString()));
			}
		}
	}
}
