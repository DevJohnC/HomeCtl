namespace HomeCtl.Kinds
{
	/// <summary>
	/// An instance of a kind.
	/// </summary>
	/// <remarks>
	/// Usage and concept are similar to a Type instance in .NET
	/// </remarks>
	public class Kind : IResource
	{
		public Kind(string kindName, string kindNamePlural, string group, string apiVersion, KindSchema schema, Kind? extendsKind)
		{
			KindName = kindName;
			KindNamePlural = kindNamePlural;
			Group = group;
			ApiVersion = apiVersion;
			Schema = schema;
			ExtendsKind = extendsKind;
		}

		/// <summary>
		/// Gets the kind's name.
		/// </summary>
		public string KindName { get; }

		/// <summary>
		/// Gets the kind's plural name.
		/// </summary>
		public string KindNamePlural { get; }

		/// <summary>
		/// Gets the group the kind belongs to.
		/// </summary>
		public string Group { get; }

		/// <summary>
		/// Gets the api version of the kind.
		/// </summary>
		public string ApiVersion { get; }

		/// <summary>
		/// Gets the schema describing the structure of the kind.
		/// </summary>
		public KindSchema Schema { get; }

		/// <summary>
		/// Gets the kind this kind extends, if it extends a kind.
		/// </summary>
		public Kind? ExtendsKind { get; }

		Kind IResource.Kind => CoreKinds.Kind;
	}
}
