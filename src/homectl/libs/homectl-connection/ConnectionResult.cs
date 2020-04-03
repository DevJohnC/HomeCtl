using System.Net.Http;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Result of a connection attempt.
	/// </summary>
	public struct ConnectionResult
	{
		/// <summary>
		/// Connection attempt failed result.
		/// </summary>
		public static readonly ConnectionResult Failed = new ConnectionResult(false, null);

		public ConnectionResult(bool wasConnectionEstablished, HttpClient? client)
		{
			WasConnectionEstablished = wasConnectionEstablished;
			Client = client;
		}

		/// <summary>
		/// Gets a value indicating if the connection attempt was successful.
		/// </summary>
		public bool WasConnectionEstablished { get; }

		/// <summary>
		/// Gets the connected HttpClient if a connection was made.
		/// </summary>
		public HttpClient? Client { get; }
	}
}
