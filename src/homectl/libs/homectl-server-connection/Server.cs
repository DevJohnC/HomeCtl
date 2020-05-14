using HomeCtl.Services.Server;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public abstract class Server
	{
		protected Server(EndpointConnectionManager connectionManager)
		{
			ConnectionManager = connectionManager;
		}

		public EndpointConnectionManager ConnectionManager { get; }

		/// <summary>
		/// Gets the local nodes IP address as seen by the API server.
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetPerceivedIpAddress()
		{
			var client = new Information.InformationClient(ConnectionManager.ServicesChannel);
			var response = await client.GetClientIpAddressAsync(Services.Empty.Instance);
			return response.IpAddress;
		}
	}
}
