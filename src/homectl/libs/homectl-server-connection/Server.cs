namespace HomeCtl.Connection
{
	public abstract class Server
	{
		protected Server(EndpointConnectionManager connectionManager)
		{
			ConnectionManager = connectionManager;
		}

		public EndpointConnectionManager ConnectionManager { get; }
	}
}
