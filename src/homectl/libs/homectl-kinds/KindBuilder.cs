using HomeCtl.Kinds.Resources;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace HomeCtl.Kinds
{
	public class KindBuilder
	{
		public static Kind<T> Build<T>(
			string groupName, string apiVersion, string kindName, string kindNamePlural,
			Func<T, ResourceDocument?> convertToDocument,
			Func<ResourceDocument, T?> convertToResource,
			Action<MetadataBuilder> metadata,
			Action<SpecBuilder>? spec = null,
			Action<StateBuilder>? state = null,
			Kind? extendsKind = null)
			where T : class, IResource
		{
			var metadataBuilder = new MetadataBuilder();
			var specBuilder = spec != null ? new SpecBuilder() : default;
			var stateBuilder = state != null ? new StateBuilder() : default;

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

		public abstract class Builder<TBuilder>
			where TBuilder : Builder<TBuilder>
		{
			private readonly TBuilder _this;

			private bool _allowAdditionalProperties;

			private readonly IDictionary<string, OpenApiSchema> _properties = new Dictionary<string, OpenApiSchema>();

			private readonly ISet<string> _requiredProperties = new HashSet<string>();

			public Builder()
			{
				_this = (TBuilder)this;
			}

			public TBuilder OptionalString(string fieldName, string? format = null)
			{
				var schema = new OpenApiSchema
				{
					Type = "string"
				};
				if (format != null)
					schema.Format = format;

				_properties.Add(fieldName, schema);
				return _this;
			}

			public TBuilder RequireString(string fieldName, string? format = null)
			{
				OptionalString(fieldName, format);
				_requiredProperties.Add(fieldName);
				return _this;
			}

			public TBuilder AllowAdditionalProperties(bool allowed = true)
			{
				_allowAdditionalProperties = allowed;
				return _this;
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

		public class MetadataBuilder : Builder<MetadataBuilder>
		{
		}

		public class SpecBuilder : Builder<SpecBuilder>
		{
		}

		public class StateBuilder : Builder<StateBuilder>
		{
		}
	}
}
