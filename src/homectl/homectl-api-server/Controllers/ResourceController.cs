using homectl.Application;
using homectl.Extensions;
using homectl.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace homectl.Controllers
{
	[ApiController]
	[Route("~/apis/{group}/{apiVersion}/{kind}")]
	public class ResourceController : Microsoft.AspNetCore.Mvc.Controller
	{
		public readonly static JsonConverter[] JsonConverters = new JsonConverter[]
		{
			new ResourceManifestJsonConverter(),
			new ResourceDetailsJsonConverter()
		};

		private ActionResult<ResourceDetails> CreateNewResource(ResourceManager resourceKind, ResourceManifest manifest, Guid? resourceIdentifier)
		{
			var resource = resourceKind.Create(manifest.Metadata, manifest.Spec, resourceIdentifier);
			if (resource == null)
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);

			return CreatedAtAction(
				nameof(GetSingle),
				new { group = resource.Kind.Group, apiVersion = resource.Kind.ApiVersion, kind = resource.Kind.KindName, identifier = resource.Record.Id },
				new ResourceDetails(resource));
		}

		private ActionResult<ResourceDetails> UpdateResource(Resource resource, ResourceManager resourceKind, ResourceManifest manifest)
		{
			var updatedResource = resourceKind.Update(resource, manifest.Metadata, manifest.Spec);
			if (updatedResource == null)
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);

			return new ResourceDetails(resource);
		}

		[HttpGet]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<IEnumerable<ResourceDetails>> GetMany(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromServices] KindManager resourceManager
			)
		{
			if (!resourceManager.TryGetKind(group, apiVersion, kind, out var resourceKind) ||
				resourceKind == null)
				return NotFound();

			var resources = resourceKind.GetAll();

			return resources.Select(q => new ResourceDetails(q)).ToList();
		}

		[HttpGet("{identifier:guid}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<ResourceDetails> GetSingle(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromRoute] Guid identifier,
			[FromServices] KindManager resourceManager
			)
		{
			if (!resourceManager.TryGetKind(group, apiVersion, kind, out var resourceKind) ||
				resourceKind == null)
				return NotFound();

			var resource = resourceKind.GetSingle(identifier);
			if (resource == null)
				return NotFound();

			return new ResourceDetails(resource);
		}

		[HttpPut]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<ResourceDetails> Save(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromBody] ResourceManifest manifest,
			[FromServices] KindManager resourceManager
			)
		{
			if (!resourceManager.TryGetKind(group, apiVersion, kind, out var resourceKind) ||
				resourceKind == null)
				return NotFound();

			resourceKind.Kind.Schema.MetadataSchema.Validate(manifest.Metadata, ModelState);
			resourceKind.Kind.Schema.SpecSchema.Validate(manifest.Spec, ModelState);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var resource = resourceKind.GetSingle(manifest.Metadata);

			if (resource == null)
			{
				return CreateNewResource(resourceKind, manifest, resourceIdentifier: null);
			}
			else
			{
				return UpdateResource(resource, resourceKind, manifest);
			}
		}

		[HttpPut("{identifier:guid}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<ResourceDetails> Save(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromRoute] Guid identifier,
			[FromBody] ResourceManifest manifest,
			[FromServices] KindManager resourceManager
			)
		{
			if (!resourceManager.TryGetKind(group, apiVersion, kind, out var resourceKind) ||
				resourceKind == null)
				return NotFound();

			resourceKind.Kind.Schema.MetadataSchema.Validate(manifest.Metadata, ModelState);
			resourceKind.Kind.Schema.SpecSchema.Validate(manifest.Spec, ModelState);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var resource = resourceKind.GetSingle(identifier);

			if (resource == null)
			{
				return CreateNewResource(resourceKind, manifest, identifier);
			}
			else
			{
				return UpdateResource(resource, resourceKind, manifest);
			}
		}

		[HttpPatch("{identifier:guid}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<ResourceDetails> UpdatePatch(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromRoute] Guid identifier,
			[FromBody] JsonPatchDocument<ResourceManifest> patchDocument,
			[FromServices] KindManager resourceManager
			)
		{
			if (!resourceManager.TryGetKind(group, apiVersion, kind, out var resourceKind) ||
				resourceKind == null)
				return NotFound();

			var resource = resourceKind.GetSingle(identifier);
			if (resource == null)
				return NotFound();

			var manifest = new ResourceManifest(resource.Metadata, resource.Spec);
			patchDocument.ApplyTo(manifest, ModelState);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			resourceKind.Kind.Schema.MetadataSchema.Validate(manifest.Metadata, ModelState);
			resourceKind.Kind.Schema.SpecSchema.Validate(manifest.Spec, ModelState);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return UpdateResource(resource, resourceKind, manifest);
		}

		[HttpDelete("{identifier:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Delete(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromRoute] Guid identifier,
			[FromServices] KindManager resourceManager
			)
		{
			if (!resourceManager.TryGetKind(group, apiVersion, kind, out var resourceKind) ||
				resourceKind == null)
				return NotFound();

			var resource = resourceKind.GetSingle(identifier);
			if (resource == null)
				return NotFound();

			resourceKind.Remove(resource);

			return Ok();
		}

		public class ResourceDetails
		{
			public ResourceDetails(Resource resource)
			{
				Group = resource.Kind.Group;
				ApiVersion = resource.Kind.ApiVersion;
				KindName = resource.Kind.KindName;
				Record = resource.Record;
				Metadata = resource.Metadata;
				Spec = resource.Spec;

				if (resource.Kind.Schema.HasStateSchema)
					State = resource.State;
			}

			public string Group { get; set; }

			public string ApiVersion { get; set; }

			public string KindName { get; set; }

			public ResourceRecord Record { get; set; }

			public ResourceMetadata Metadata { get; set; }

			public ResourceSpec Spec { get; set; }

			public ResourceState? State { get; set; }
		}

		public class ResourceManifest
		{
			public ResourceManifest(ResourceMetadata metadata, ResourceSpec spec)
			{
				Metadata = metadata;
				Spec = spec;
			}

			public ResourceMetadata Metadata { get; set; }

			public ResourceSpec Spec { get; set; } 
		}

		private class ResourceManifestJsonConverter : JsonConverter<ResourceManifest>
		{
			public override ResourceManifest ReadJson(JsonReader reader, Type objectType, ResourceManifest existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var jsonObj = JToken.ReadFrom(reader);
				return new ResourceManifest(
					ResourceMetadata.FromJson(jsonObj["metadata"]),
					ResourceSpec.FromJson(jsonObj["spec"]));
			}

			public override void WriteJson(JsonWriter writer, ResourceManifest value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}

		private class ResourceDetailsJsonConverter : JsonConverter<ResourceDetails>
		{
			public override ResourceDetails ReadJson(JsonReader reader, Type objectType, ResourceDetails existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override void WriteJson(JsonWriter writer, ResourceDetails value, JsonSerializer serializer)
			{
				writer.WriteStartObject();

				writer.WritePropertyName(nameof(value.ApiVersion));
				writer.WriteValue(value.ApiVersion);

				writer.WritePropertyName(nameof(value.KindName));
				writer.WriteValue(value.KindName);

				writer.WritePropertyName(nameof(value.Record));
				serializer.Serialize(writer, value.Record);

				writer.WritePropertyName(nameof(value.Metadata));
				serializer.Serialize(writer, ((IExpandoDocument)value.Metadata).Document);

				writer.WritePropertyName(nameof(value.Spec));
				serializer.Serialize(writer, ((IExpandoDocument)value.Spec).Document);

				if (value.State != null)
				{
					writer.WritePropertyName(nameof(value.State));
					serializer.Serialize(writer, ((IExpandoDocument)value.State).Document);
				}

				writer.WriteEndObject();
			}
		}
	}
}
