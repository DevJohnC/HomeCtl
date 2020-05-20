using HomeCtl.Kinds.Resources;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCtl.Kinds
{
	public class KindBuilder
	{
		public static Kind<T> Build<T>(
			string groupName, string apiVersion, string kindName, string kindNamePlural,
			Func<T, ResourceDocument?> convertToDocument,
			Func<ResourceDocument, T?> convertToResource,
			Action<Builder> metadata,
			Action<Builder>? spec = null,
			Action<Builder>? state = null,
			Kind? extendsKind = null)
			where T : class, IResource
		{
			var metadataBuilder = new Builder();
			var specBuilder = spec != null ? new Builder() : default;
			var stateBuilder = state != null ? new Builder() : default;

			metadata(metadataBuilder);
#pragma warning disable CS8604 // Possible null reference argument.
			spec?.Invoke(specBuilder);
			state?.Invoke(stateBuilder);
#pragma warning restore CS8604 // Possible null reference argument.

			var schema = new KindSchema(
				metadataBuilder.BuildSchema(),
				specBuilder?.BuildSchema(),
				stateBuilder?.BuildSchema()
				);

			return new Kind<T>(kindName, kindNamePlural, groupName, apiVersion,
				schema, extendsKind, convertToDocument, convertToResource);
		}

		public class Builder
		{
			private bool _allowAdditionalProperties;

			private readonly IDictionary<string, OpenApiSchema> _properties = new Dictionary<string, OpenApiSchema>();

			private readonly ISet<string> _requiredProperties = new HashSet<string>();

			public Builder Optional(string fieldName, OpenApiSchema typeSchema)
			{
				_properties.Add(fieldName, typeSchema);
				return this;
			}

			public Builder Required(string fieldName, OpenApiSchema typeSchema)
			{
				_properties.Add(fieldName, typeSchema);
				_requiredProperties.Add(fieldName);
				return this;
			}

			private OpenApiSchema ObjectSchema(Action<Builder> builderAction)
			{
				var builder = new Builder();
				builderAction?.Invoke(builder);
				return builder.BuildSchema();
			}

			private OpenApiSchema ArraySchema(OpenApiSchema typeSchema)
			{
				return new OpenApiSchema
				{
					Type = "array",
					Items = typeSchema
				};
			}

			public Builder OptionalObject(string fieldName, Action<Builder> builderAction)
			{
				return Optional(fieldName, ObjectSchema(builderAction));
			}

			public Builder RequireObject(string fieldName, Action<Builder> builderAction)
			{
				return Required(fieldName, ObjectSchema(builderAction));
			}

			public Builder OptionalObjectArray(string fieldName, Action<Builder> builderAction)
			{
				return Optional(fieldName, ArraySchema(ObjectSchema(builderAction)));
			}

			public Builder RequireObjectArray(string fieldName, Action<Builder> builderAction)
			{
				return Required(fieldName, ArraySchema(ObjectSchema(builderAction)));
			}

			private OpenApiSchema EnumSchema(string[] possibleValues)
			{
				return new OpenApiSchema
				{
					Type = "string",
					Enum = possibleValues.Select(q => new OpenApiString(q)).ToList<IOpenApiAny>()
				};
			}

			public Builder OptionalEnum<T>(string fieldName) where T : Enum
			{
				return OptionalEnum(fieldName, Enum.GetNames(typeof(T)));
			}

			public Builder RequireEnum<T>(string fieldName) where T : Enum
			{
				return RequireEnum(fieldName, Enum.GetNames(typeof(T)));
			}

			public Builder OptionalEnum(string fieldName, params string[] possibleValues)
			{
				return Optional(fieldName, EnumSchema(possibleValues));
			}

			public Builder RequireEnum(string fieldName, params string[] possibleValues)
			{
				return Required(fieldName, EnumSchema(possibleValues));
			}

			private OpenApiSchema StringSchema(string? format)
			{
				var schema = new OpenApiSchema
				{
					Type = "string"
				};
				if (format != null)
					schema.Format = format;

				return schema;
			}

			public Builder OptionalString(string fieldName, string? format = null)
			{
				return Optional(fieldName, StringSchema(format));
			}

			public Builder RequireString(string fieldName, string? format = null)
			{
				return Required(fieldName, StringSchema(format));
			}

			public Builder RequireStringArray(string fieldName, string? format = null)
			{
				return Required(fieldName, ArraySchema(StringSchema(format)));
			}

			public Builder AllowAdditionalProperties(bool allowed = true)
			{
				_allowAdditionalProperties = allowed;
				return this;
			}

			public OpenApiSchema BuildSchema()
			{
				return new OpenApiSchema
				{
					Type = "object",
					AdditionalPropertiesAllowed = _allowAdditionalProperties,
					Properties = _properties,
					Required = _requiredProperties
				};
			}
		}
	}
}
