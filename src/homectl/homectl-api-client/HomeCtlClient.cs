using Grpc.Net.Client;
using System.Net.Http;

namespace homectl
{
	public class HomeCtlClient
	{
		public HomeCtlClient(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public HttpClient HttpClient { get; }

		private GrpcChannel? _grpcChannel;
		public GrpcChannel GrpcChannel => _grpcChannel ?? CreateGrpcChannel();

		public ResourceClient<TResource> GetResourceClient<TResource>(ResourceType<TResource> resourceType)
		{
			return default;
		}

		private GrpcChannel CreateGrpcChannel()
		{
			_grpcChannel = GrpcChannel.ForAddress(
				HttpClient.BaseAddress,
				new GrpcChannelOptions { HttpClient = HttpClient, DisposeHttpClient = false }
				);
			return _grpcChannel;
		}
	}
}
