using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	class ResourceStateManager
	{
		private readonly ResourceManagerContainer _resourceManagerAccessor;

		public ResourceStateManager(ResourceManagerContainer resourceManagerAccessor)
		{
			_resourceManagerAccessor = resourceManagerAccessor;
		}

		private ResourceDocument CreateBlankPatchDocument(IResource resource)
		{
			return new ResourceDocument(
				new ResourceDefinition(new List<ResourceField>
				{
					new ResourceField("identity", ResourceFieldValue.String(resource.GetIdentity()))
				}));
		}

		private Task ApplyPatch(ResourceDocument patchDocument)
		{
			return Task.CompletedTask;
		}

		public Task PatchDefinition(IResource resource, ResourceDefinition definitionPatch)
		{
			var patchDoc = CreateBlankPatchDocument(resource);
			patchDoc.Definition = definitionPatch;
			return ApplyPatch(patchDoc);
		}

		public Task PatchMetadata(IResource resource, ResourceMetadata metadataPatch)
		{
			var patchDoc = CreateBlankPatchDocument(resource);
			patchDoc.Metadata = metadataPatch;
			return ApplyPatch(patchDoc);
		}

		public Task PatchSpec(IResource resource, ResourceSpec specPatch)
		{
			var patchDoc = CreateBlankPatchDocument(resource);
			patchDoc.Spec = specPatch;
			return ApplyPatch(patchDoc);
		}

		public Task PatchState(IResource resource, HomeCtl.Kinds.Resources.ResourceState statePatch)
		{
			var patchDoc = CreateBlankPatchDocument(resource);
			patchDoc.State = statePatch;
			return ApplyPatch(patchDoc);
		}
	}
}
