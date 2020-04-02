using homectl.Application;
using homectl.Extensions;
using homectl.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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
		private ActionResult<ResourceDetails> CreateNewResource(ResourceManager resourceKind, ResourceManifest manifest, Guid? resourceIdentifier)
		{
			if (!resourceKind.TryCreate(manifest.Metadata, manifest.Spec, resourceIdentifier, out var resource))
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);

			return CreatedAtAction(
				nameof(GetSingle),
				new { group = resourceKind.Kind.Group, apiVersion = resourceKind.Kind.ApiVersion, kind = resourceKind.Kind.KindName, identifier = resource.Record.Id },
				new ResourceDetails(resource, resourceKind.Kind));
		}

		private ActionResult<ResourceDetails> UpdateResource(ResourceManager.ResourceRecordPair resource, ResourceManager resourceKind, ResourceManifest manifest)
		{
			if (resourceKind.TryUpdate(resource, manifest.Metadata, manifest.Spec, out var updatedResource))
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);

			return new ResourceDetails(resource, resourceKind.Kind);
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

			return resources.Select(q => new ResourceDetails(q, resourceKind.Kind)).ToList();
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

			if (!resourceKind.TryGetSingle(identifier, out var resource))
				return NotFound();

			return new ResourceDetails(resource, resourceKind.Kind);
		}

		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<ResourceDetails> Create(
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

			return CreateNewResource(resourceKind, manifest, resourceIdentifier: null);
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

			if (resourceKind.TryGetSingle(identifier, out var resource))
			{
				return UpdateResource(resource, resourceKind, manifest);
			}
			else
			{
				return CreateNewResource(resourceKind, manifest, identifier);
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

			if (!resourceKind.TryGetSingle(identifier, out var resource))
				return NotFound();

			if (!resourceKind.Kind.TryGetMetadata(resource.Resource, out var metadata) ||
				!resourceKind.Kind.TryGetSpec(resource.Resource, out var spec))
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);

			//var manifest = new ResourceManifest
			//{
			//	Manifest = metadata,
			//	Spec = spec
			//};
			//patchDocument.ApplyTo(manifest, ModelState);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//resourceKind.Kind.Schema.MetadataSchema.Validate(manifest.Metadata, ModelState);
			//resourceKind.Kind.Schema.SpecSchema.Validate(manifest.Spec, ModelState);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			throw new NotImplementedException();
			//return UpdateResource(resource, resourceKind, manifest);
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

			if (!resourceKind.TryGetSingle(identifier, out var resource))
				return NotFound();

			resourceKind.Remove(resource.Record);

			return Ok();
		}

		public class ResourceDetails
		{
			public ResourceDetails(ResourceManager.ResourceRecordPair resource,
				KindDescriptor kind)
			{
				Group = kind.Group;
				ApiVersion = kind.ApiVersion;
				KindName = kind.KindName;
				Record = resource.Record;

				if (kind.TryGetMetadata(resource.Resource, out var metadata))
					Metadata = metadata;
				else
					Metadata = new object();

				if (kind.TryGetSpec(resource.Resource, out var spec))
					Spec = spec;
				else
					Spec = new object();

				if (kind.Schema.HasStateSchema)
				{
					if (kind.TryGetState(resource.Resource, out var state))
						State = state;
					else
						State = new object();
				}
			}

			public string Group { get; set; }

			public string ApiVersion { get; set; }

			public string KindName { get; set; }

			public ResourceRecord Record { get; set; }

			public object Metadata { get; set; }

			public object Spec { get; set; }

			public object? State { get; set; }
		}

		public class ResourceManifest
		{
			public JObject Metadata { get; set; } = new JObject();

			public JObject Spec { get; set; } = new JObject();
		}
	}
}
