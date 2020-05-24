namespace HomeCtl.Kinds
{
	/// <summary>
	/// An instance of a kind.
	/// </summary>
	/// <remarks>
	/// If a kind is analagous to a .NET type, a resource is analagous to an instance of a type.
	/// </remarks>
	public interface IResource
	{
		/// <summary>
		/// Gets the kind this resource is an instance of.
		/// </summary>
		Kind Kind { get; }

		/// <summary>
		/// Gets the unique identity of the resource.
		/// </summary>
		/// <returns></returns>
		string GetIdentity();
	}
}
