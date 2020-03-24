using homectl_api_server.Application;
using homectl_api_server.Extensions;
using homectl_api_server.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace homectl_api_server.Controllers
{
	[ApiController]
	[Route("~/apis/{group}/{apiVersion}/{kind}")]
	public class ResourceController : Microsoft.AspNetCore.Mvc.Controller
	{
		public readonly static JsonConverter[] JsonConverters = new JsonConverter[]
		{
			new ResourceManifestJsonConverter()
		};

		[HttpGet]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<IEnumerable<ResourceDetails>> GetMany(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromServices] ResourceManager resourceManager
			)
		{
			var resourceKind = resourceManager.GetKind(
				group, apiVersion, kind);

			if (resourceKind == KindManager.Nothing)
				return NotFound();

			var resources = resourceKind.GetAll();

			return resources.Select(q => new ResourceDetails(
				$"{resourceKind.Kind.Group}/{resourceKind.Kind.ApiVersion}",
				resourceKind.Kind.KindName, q.Metadata,
				q.Spec, q.State
			)).ToList();
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
			[FromServices] ResourceManager resourceManager
			)
		{
			var resourceKind = resourceManager.GetKind(
				group, apiVersion, kind);

			if (resourceKind == KindManager.Nothing)
				return NotFound();

			var resource = resourceKind.GetSingle(identifier);
			if (resource == Resource.Nothing)
				return NotFound();

			return new ResourceDetails
			(
				$"{resourceKind.Kind.Group}/{resourceKind.Kind.ApiVersion}",
				resourceKind.Kind.KindName, resource.Metadata,
				resource.Spec, resource.State
			);
		}

		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<ResourceDetails> Create(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromBody] ResourceManifest manifest,
			[FromServices] ResourceManager resourceManager
			)
		{
			var resourceKind = resourceManager.GetKind(
				group, apiVersion, kind);

			if (resourceKind == KindManager.Nothing)
				return NotFound();

			//  todo: get valiation and creation errors out of this API call and return them to the caller on failure
			if (!resourceKind.Kind.Schema.Spec.Validate(manifest.Spec, ModelState))
				return BadRequest(ModelState);

			var resource = resourceKind.Create(manifest.Metadata, manifest.Spec);
			if (resource == Resource.Nothing)
				return NotFound();

			return CreatedAtAction(nameof(GetSingle), new { identifier = resource.Metadata.Id }, new ResourceDetails(
				$"{resourceKind.Kind.Group}/{resourceKind.Kind.ApiVersion}",
				resourceKind.Kind.KindName, resource.Metadata,
				resource.Spec, resource.State
			));
		}

		[HttpPut("{identifier:guid}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<ResourceDetails> UpdateFull(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromRoute] Guid identifier,
			[FromBody] ResourceManifest manifest,
			[FromServices] ResourceManager resourceManager
			)
		{
			var resourceKind = resourceManager.GetKind(
				group, apiVersion, kind);

			if (resourceKind == KindManager.Nothing)
				return NotFound();

			var resource = resourceKind.GetSingle(identifier);
			if (resource == Resource.Nothing)
				return NotFound();

			if (!resourceKind.Kind.Schema.Spec.Validate(manifest.Spec, ModelState))
				return BadRequest(ModelState);

			resourceKind.UpdateSpec(resource, manifest.Metadata, manifest.Spec);

			return new ResourceDetails($"{resourceKind.Kind.Group}/{resourceKind.Kind.ApiVersion}",
				resourceKind.Kind.KindName, resource.Metadata, resource.Spec,
				resource.State);
		}

		[HttpPatch("{identifier:guid}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<ResourceDetails> UpdatePatch(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromRoute] Guid identifier,
			[FromBody] JsonPatchDocument<ResourceManifest> patchDocument,
			[FromServices] ResourceManager resourceManager
			)
		{
			var resourceKind = resourceManager.GetKind(
				group, apiVersion, kind);

			if (resourceKind == KindManager.Nothing)
				return NotFound();

			var resource = resourceKind.GetSingle(identifier);
			if (resource == Resource.Nothing)
				return NotFound();

			var manifest = new ResourceManifest(resource.Metadata, resource.Spec);
			patchDocument.ApplyTo(manifest, ModelState);
			if (!ModelState.IsValid || !resourceKind.Kind.Schema.Spec.Validate(manifest.Spec, ModelState))
				return BadRequest(ModelState);

			resourceKind.UpdateSpec(resource, manifest.Metadata, manifest.Spec);

			return new ResourceDetails($"{resourceKind.Kind.Group}/{resourceKind.Kind.ApiVersion}", resourceKind.Kind.KindName,
				resource.Metadata, resource.Spec, resource.State);
		}

		[HttpDelete("{identifier:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Delete(
			[FromRoute] string group,
			[FromRoute] string apiVersion,
			[FromRoute] string kind,
			[FromRoute] Guid identifier,
			[FromServices] ResourceManager resourceManager
			)
		{
			var resourceKind = resourceManager.GetKind(
				group, apiVersion, kind);

			if (resourceKind == KindManager.Nothing)
				return NotFound();

			var resource = resourceKind.GetSingle(identifier);
			if (resource == Resource.Nothing)
				return NotFound();

			resourceKind.Remove(resource);

			return Ok();
		}

		public class ResourceDetails
		{
			public ResourceDetails(string apiVersion, string kind, ResourceMetadata metadata, ResourceSpec spec, ResourceState state)
			{
				ApiVersion = apiVersion;
				Kind = kind;
				Metadata = metadata;
				Spec = spec;
				State = state;
			}

			public string ApiVersion { get; set; }

			public string Kind { get; set; }

			public ResourceMetadata Metadata { get; set; }

			public ResourceSpec Spec { get; set; }

			public ResourceState State { get; set; }
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

		public class ResourceManifestJsonConverter : JsonConverter<ResourceManifest>
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
	}
}
