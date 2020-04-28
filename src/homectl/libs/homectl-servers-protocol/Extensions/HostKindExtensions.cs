using HomeCtl.Clients;

namespace HomeCtl.Kinds
{
	/// <summary>
	/// Extension methods for the Host kind type.
	/// </summary>
	public static class HostKindExtensions
	{
		/// <summary>
		/// Determine if the host matches the provided metadata query.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="metadataQuery"></param>
		/// <returns></returns>
		public static bool MatchesQuery(this Host host, MetadataQuery metadataQuery)
		{
			switch (metadataQuery.MatchOperation)
			{
				case MatchOperation.And:
					foreach (var field in metadataQuery.FieldQueries)
					{
						if (!TryGetMetadataFieldValue(host, field.FieldName, out var fieldValue) ||
							!MatchesQuery(fieldValue, field))
							return false;
					}
					return true;
				case MatchOperation.Or:
					foreach (var field in metadataQuery.FieldQueries)
					{
						if (!TryGetMetadataFieldValue(host, field.FieldName, out var fieldValue))
							continue;

						if (MatchesQuery(fieldValue, field))
							return true;
					}
					return false;
				default:
					return false;
			}
		}

		/// <summary>
		/// Test a field's value against a field query.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		private static bool MatchesQuery(string value, MetadataFieldQuery query)
		{
			switch (query.MatchType)
			{
				case MatchType.Exact:
					return string.Equals(value, query.FieldValue, System.StringComparison.Ordinal);
				case MatchType.CaseInsensitiveMatch:
					return string.Equals(value, query.FieldValue, System.StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		/// <summary>
		/// Attempt to read metadata field value on a given host instance.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool TryGetMetadataFieldValue(Host host, string fieldName, out string value)
		{
			switch (fieldName)
			{
				case nameof(Host.HostMetadata.Hostname):
					value = host.Metadata.Hostname;
					return true;
				case nameof(Host.HostMetadata.Name):
					value = host.Metadata.Name;
					return true;
				default:
					value = string.Empty;
					return false;
			}
		}
	}
}
