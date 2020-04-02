using System;

namespace homectl.Resources
{
	public abstract class KindDescriptor
	{
		protected KindDescriptor(Guid kindId, string group, string apiVersion, string kindName, string kindNamePlural, KindSchema schema)
		{
			KindId = kindId;
			Group = group;
			ApiVersion = apiVersion;
			KindName = kindName;
			KindNamePlural = kindNamePlural;
			Schema = schema;
		}

		protected KindDescriptor(Guid kindId, string group, string apiVersion, string kindName, string kindNamePlural, KindSchema schema,
			KindDescriptor extendsKind) : 
			this(kindId, group, apiVersion, kindName, kindNamePlural, schema)
		{
			ExtendsKind = extendsKind;
		}

		public Guid KindId { get; }

		public string Group { get; }

		public string ApiVersion { get; }

		public string KindName { get; }

		public string KindNamePlural { get; }

		public KindSchema Schema { get; }

		public KindDescriptor ExtendsKind { get; }

		public abstract bool TryGetMetadata(IResource resource, out object metadata);

		public abstract bool TryGetSpec(IResource resource, out object spec);

		public abstract bool TryGetState(IResource resource, out object state);
	}

	public class TypeDescriptor<TKind> : KindDescriptor
		where TKind : class, IResource
	{
		private readonly Func<TKind, object> _metadataFunc;
		private readonly Func<TKind, object> _specFunc;
		private readonly Func<TKind, object> _stateFunc;

		public TypeDescriptor(Guid kindId, string group, string apiVersion, string kindName, string kindNamePlural,
			KindSchema schema, Func<TKind, object> metadataFunc, Func<TKind, object> specFunc, Func<TKind, object> stateFunc = null) :
			base(kindId, group, apiVersion, kindName, kindNamePlural, schema)
		{
			_metadataFunc = metadataFunc;
			_specFunc = specFunc;
			_stateFunc = stateFunc;
		}

		public override bool TryGetMetadata(IResource resource, out object metadata)
		{
			if (resource is TKind resourceOfKind)
			{
				metadata = _metadataFunc(resourceOfKind);
				return true;
			}
			else
			{
				metadata = default;
				return false;
			}
		}

		public override bool TryGetSpec(IResource resource, out object spec)
		{
			if (resource is TKind resourceOfKind)
			{
				spec = _specFunc(resourceOfKind);
				return true;
			}
			else
			{
				spec = default;
				return false;
			}
		}

		public override bool TryGetState(IResource resource, out object state)
		{
			if (resource is TKind resourceOfKind)
			{
				state = _stateFunc(resourceOfKind);
				return true;
			}
			else
			{
				state = default;
				return false;
			}
		}
	}
}
