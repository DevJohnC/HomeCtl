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

		public KindManager() : base(CoreKinds.Kind)
		{
			Add(this);
			CreateCoreKinds();
		}

		private readonly Dictionary<(string group, string apiVersion, string kindName), ResourceManager> _kinds =
			new Dictionary<(string group, string apiVersion, string kindName), ResourceManager>();

		private void CreateCoreKinds()
		{
			Add(new HostManager());
		}

		private (string group, string apiVersion, string kindName) CreateKey(KindDescriptor kind)
		{
			return CreateKey(kind.Group, kind.ApiVersion, kind.KindName);
		}

		private (string group, string apiVersion, string kindName) CreateKey(string group, string apiVersion, string kindName)
		{
			return (group, apiVersion, kindName);
		}

		private void Add(ResourceManager manager)
		{
			var key = CreateKey(manager.Kind);
			_kinds.Add(key, manager);
			Add(new ResourceRecord(manager.Kind.KindId), new KindResource(manager.Kind));
		}

		public bool TryGetKind(string group, string apiVersion, string kindName, out ResourceManager? resourceManager)
		{
			var key = CreateKey(group, apiVersion, kindName);
			return _kinds.TryGetValue(key, out resourceManager);
		}
	}
}
