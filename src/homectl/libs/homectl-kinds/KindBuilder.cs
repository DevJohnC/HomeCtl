using HomeCtl.Kinds.Resources;
using Microsoft.OpenApi.Models;
using System;

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
			var specBuilder = new SpecBuilder();
			var stateBuilder = new StateBuilder();

			metadata(metadataBuilder);
			spec?.Invoke(specBuilder);
			state?.Invoke(stateBuilder);

			var schema = new KindSchema(
				metadataBuilder.BuildSchema(),
				specBuilder.BuildSchema(),
				stateBuilder.BuildSchema()
				);

			return new Kind<T>(kindName, kindNamePlural, groupName, apiVersion,
				schema, extendsKind, convertToDocument, convertToResource);
		}

		public abstract class Builder
		{
			public OpenApiSchema BuildSchema()
			{
				return new OpenApiSchema();
			}
		}

		public class MetadataBuilder : Builder
		{
		}

		public class SpecBuilder : Builder
		{
		}

		public class StateBuilder : Builder
		{
		}
	}
}
