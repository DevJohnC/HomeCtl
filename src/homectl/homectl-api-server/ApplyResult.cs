using System.Collections.Generic;

namespace HomeCtl.ApiServer
{
	/// <summary>
	/// The result of calling an Apply method on a resource manager.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct ApplyResult<T>
	{
		private readonly static T[] _empty = new T[0];

		public ApplyResult(IReadOnlyList<T>? created = null, IReadOnlyList<T>? updated = null, IReadOnlyList<T>? deleted = null)
		{
			Created = created ?? _empty;
			Updated = updated ?? _empty;
			Deleted = deleted ?? _empty;
		}

		/// <summary>
		/// Gets a collection of resources that were created.
		/// </summary>
		public IReadOnlyList<T> Created { get; }

		/// <summary>
		/// Gets a collection of resources that were updated.
		/// </summary>
		public IReadOnlyList<T> Updated { get; }

		/// <summary>
		/// Gets a collection of resources that were deleted.
		/// </summary>
		public IReadOnlyList<T> Deleted { get; }
	}
}
