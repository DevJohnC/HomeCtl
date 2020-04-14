using HomeCtl.Kinds;
using System;

namespace HomeCtl.ApiServer.Orchestration
{
	interface IResourceFilter
	{
		bool MatchesFilter(IResource resource);
	}

	class CompositeResourceFilter : IResourceFilter
	{
		private readonly IResourceFilter[] _filters;

		public CompositeResourceFilter(params IResourceFilter[] filters)
		{
			_filters = filters;
		}

		public bool MatchesFilter(IResource resource)
		{
			foreach (var filter in _filters)
			{
				if (!filter.MatchesFilter(resource))
				{
					return false;
				}
			}
			return true;
		}
	}

	class MatchResourceIdFilter : IResourceFilter
	{
		private readonly Guid _resourceId;

		public MatchResourceIdFilter(Guid resourceId)
		{
			_resourceId = resourceId;
		}

		public bool MatchesFilter(IResource resource)
		{
			throw new System.NotImplementedException();
		}
	}

	class IsKindFilter : IResourceFilter
	{
		private readonly Kind _kind;

		public IsKindFilter(Kind kind)
		{
			_kind = kind;
		}

		public bool MatchesFilter(IResource resource)
		{
			throw new NotImplementedException();
		}
	}

	class IsNotKindFilter : IResourceFilter
	{
		private readonly Kind _kind;

		public IsNotKindFilter(Kind kind)
		{
			_kind = kind;
		}

		public bool MatchesFilter(IResource resource)
		{
			throw new NotImplementedException();
		}
	}
}
