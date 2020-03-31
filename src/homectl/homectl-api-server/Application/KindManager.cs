using homectl.Resources;
using System;
using System.Collections.Generic;

namespace homectl.Application
{
	/// <summary>
	/// Manages kind resources.
	/// </summary>
	public class KindManager : ResourceManager
	{
		public const string KIND_GROUP_CORE = "core";
		public const string KIND_VERSION_V1ALPHA1 = "v1alpha1";

		public KindManager() :
			base(Kind.CreateKindKind())
		{
			Add(Kind, this);
			CreateCoreKinds();
		}

		private readonly Dictionary<(string group, string apiVersion, string kindName), ResourceManager> _kinds =
			new Dictionary<(string group, string apiVersion, string kindName), ResourceManager>();

		private void CreateCoreKinds()
		{
			CreateHostKind();
		}

		private void CreateHostKind()
		{
			var kind = new Kind(Kind, new ResourceRecord(Host.KIND_RECORD_ID), Host.DESCRIPTOR, ResourceState.Nothing);
			var manager = new HostResourceManager(kind);
			Add(kind, manager);
		}

		private (string group, string apiVersion, string kindName) CreateKey(Kind kind)
		{
			return CreateKey(kind.Group, kind.ApiVersion, kind.KindName);
		}

		private (string group, string apiVersion, string kindName) CreateKey(string group, string apiVersion, string kindName)
		{
			return (group, apiVersion, kindName);
		}

		private void Add(Kind kind, ResourceManager manager)
		{
			var key = CreateKey(kind);
			_kinds.Add(key, manager);
			Add(kind);
		}

		public bool TryGetKind(string group, string apiVersion, string kindName, out ResourceManager? resourceManager)
		{
			var key = CreateKey(group, apiVersion, kindName);
			return _kinds.TryGetValue(key, out resourceManager);
		}

		public override Resource? Update(Resource resource, ResourceMetadata metadata, ResourceSpec spec)
		{
			throw new NotSupportedException("Updating kinds is not permitted.");
		}
	}
}
