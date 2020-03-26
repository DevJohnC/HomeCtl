using System;

namespace homectl_api_client
{
	public class ConnectionManager
	{
		public event EventHandler<ConnectionEventArgs> Connected;
	}

	public class ConnectionEventArgs : EventArgs
	{
		public ConnectionEventArgs(HomeCtlClient client)
		{
			Client = client;
		}

		public HomeCtlClient Client { get; }
	}
}
