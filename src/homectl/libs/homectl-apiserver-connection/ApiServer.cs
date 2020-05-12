using HomeCtl.Events;
using HomeCtl.Services;
using HomeCtl.Services.Server;

namespace HomeCtl.Connection
{
	public class ApiServer : Server
	{
		public ApiServer(EndpointConnectionManager connectionManager, EventBus eventBus) :
			base(connectionManager)
		{
			RegisterEventHandlers(eventBus);
		}

		public ApiServerVersion ServerVersion { get; private set; }

		private void RegisterEventHandlers(EventBus eventBus)
		{
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(Handle_NewConnection);
		}

		private async void Handle_NewConnection(EndpointConnectionEvents.Connected connectedArgs)
		{
			try
			{
				var client = new Information.InformationClient(ConnectionManager.ServicesChannel);
				var version = await client.GetServerVersionAsync(Empty.Instance);
				ServerVersion = new ApiServerVersion(version.ApiServerVersion.Major, version.ApiServerVersion.Minor,
					version.ApiServerVersion.Name);
			}
			catch
			{
				//  handle exception, log etc.
			}
		}
	}
}
