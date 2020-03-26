using homectl_api_client.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace homectl
{
	public class Host
	{
		public Host(
			ConnectionManager connectionManager,
			IEnumerable<Controller> controllers
			)
		{
			ConnectionManager = connectionManager;
			Controllers = controllers.ToArray();

			AttachEventHandlers(ConnectionManager);
		}

		private void AttachEventHandlers(ConnectionManager connectionManager)
		{
			connectionManager.Connected += ConnectionManager_Connected;
		}

		private void DetachEventHandlers(ConnectionManager connectionManager)
		{
			connectionManager.Connected -= ConnectionManager_Connected;
		}

		private void ConnectionManager_Connected(object sender, ConnectionEventArgs e)
		{
			//  ensure schema exists on server?

			//  connect to grpc event stream
		}

		public ConnectionManager ConnectionManager { get; }
		public Controller[] Controllers { get; }
	}
}
