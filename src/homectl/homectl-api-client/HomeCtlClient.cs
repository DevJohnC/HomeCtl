using Grpc.Net.Client;
using System.Net.Http;

namespace homectl_api_client
{
	public class HomeCtlClient
	{
		public HomeCtlClient(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public HttpClient HttpClient { get; }

		private GrpcChannel _grpcChannel;
		public GrpcChannel GrpcChannel => _grpcChannel ?? CreateGrpcChannel();

		private GrpcChannel CreateGrpcChannel()
		{
			_grpcChannel = GrpcChannel.ForAddress(
				HttpClient.BaseAddress,
				new GrpcChannelOptions { HttpClient = HttpClient }
				);
			return _grpcChannel;
		}
	}
}
