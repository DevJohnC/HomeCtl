using homectl_api_client.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace homectl_api_client
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
		}

		public ConnectionManager ConnectionManager { get; }
		public Controller[] Controllers { get; }
	}
}
