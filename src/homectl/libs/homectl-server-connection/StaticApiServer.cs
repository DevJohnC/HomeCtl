using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public class StaticApiServer : IServerEndpointProvider
	{
		public StaticApiServer(ServerEndpoint apiServerEndpoint)
		{
			Endpoint = apiServerEndpoint;
			_taskResult = Task.FromResult(apiServerEndpoint);
		}

		public ServerEndpoint Endpoint { get; }

		private readonly Task<ServerEndpoint> _taskResult;

		public Task<ServerEndpoint> GetServerEndpoint(CancellationToken stoppingToken)
		{
			return _taskResult;
		}

		public static StaticApiServer AnyOnUri(Uri uri)
		{
			return new StaticApiServer(new ServerEndpoint(
				uri, AnyServerIdentityPolicy.Instance
				));
		}

		public static StaticApiServer AnyOnUri(string uriString)
		{
			return AnyOnUri(new Uri(uriString));
		}
	}
}
