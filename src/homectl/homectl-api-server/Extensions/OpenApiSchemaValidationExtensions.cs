using homectl.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using System;

namespace homectl.Extensions
{
	public static class OpenApiSchemaValidationExtensions
	{
		public static bool Validate(this OpenApiSchema schema, IExpandoDocument document, ModelStateDictionary modelState)
		{
			//  todo: perform validation
			return true;
			//return schema.Validate(document.Document, modelState);
		}

		private static bool Validate(this OpenApiSchema schema, JToken json, ModelStateDictionary modelState,
			string propertyName = "")
		{
			var isValid = true;

			switch (schema.Type)
			{
				//  special internal type that is satisfied with any sort of data
				case "any":
					if (!schema.ValidateAny(json, modelState, propertyName))
						isValid = false;
					break;
				case "object":
					if (!schema.ValidateObject(json, modelState, propertyName))
						isValid = false;
					break;
				case "string":
					if (!schema.ValidateString(json, modelState, propertyName))
						isValid = false;
					break;
				default:
					isValid = false;
					modelState.AddModelError(propertyName, "Invalid schema type");
					break;
			}

			return isValid;
		}

		private static bool ValidateAny(this OpenApiSchema schema, JToken json, ModelStateDictionary modelState, string propertyName)
		{
			return json.Type == JTokenType.Object || json.Type == JTokenType.Array ||
				json.Type == JTokenType.Integer || json.Type == JTokenType.Float ||
				json.Type == JTokenType.Date || json.Type == JTokenType.TimeSpan ||
				json.Type == JTokenType.String || json.Type == JTokenType.Boolean ||
				json.Type == JTokenType.Bytes || json.Type == JTokenType.Guid ||
				json.Type == JTokenType.Uri;

		}

		private static bool ValidateObject(this OpenApiSchema schema, JToken json, ModelStateDictionary modelState, string propertyName)
		{
			if (json.Type != JTokenType.Object)
			{
				modelState.AddModelError(propertyName, "Object expected");
				return false;
			}

			var isValid = true;

			foreach (JProperty property in json)
			{
				if (!schema.ValidateProperty(property, modelState, propertyName))
				{
					isValid = false;
				}
			}

			var requiredSet = schema.Required;
			if (requiredSet != null)
			{
				foreach (var requiredPropertyName in requiredSet)
				{
					var found = false;
					foreach (JProperty property in json)
					{
						if (property.Name == requiredPropertyName)
						{
							found = true;
							break;
						}
					}

					if (!found)
					{
						isValid = false;
						modelState.AddModelError(propertyName, $"Required property '{requiredPropertyName}' is missing");
					}
				}
			}

			return isValid;
		}

		private static bool ValidateString(this OpenApiSchema schema, JToken json, ModelStateDictionary modelState, string propertyName)
		{
			if (json.Type != JTokenType.String)
			{
				modelState.AddModelError(propertyName, "String expected");
				return false;
			}

			if (!string.IsNullOrWhiteSpace(schema.Format) && !ValidateStringFormat(json.Value<string>(), schema.Format))
			{
				modelState.AddModelError(propertyName, "Invalid string format");
				return false;
			}

			return true;
		}

		private static bool ValidateStringFormat(string value, string format)
		{
			switch (format)
			{
				case "uuid":
					return Guid.TryParse(value, out var _);
				default:
					return false;
			}
		}

		private static bool ValidateProperty(this OpenApiSchema schema, JProperty property, ModelStateDictionary modelState,
			string parentPropertyName)
		{
			var fullPropertyName = 
				(parentPropertyName == "") ? property.Name : $"{parentPropertyName}.{property.Name}";
			if (!schema.Properties.TryGetValue(property.Name, out var propertySchema))
			{
				modelState.AddModelError(fullPropertyName, "Field not present in schema");
				return false;
			}

			return propertySchema.Validate(property.Value, modelState, fullPropertyName);
		}
	}
}
