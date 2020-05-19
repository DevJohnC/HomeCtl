using Grpc.Core;
using HomeCtl.Services;
using HomeCtl.Services.Server;
using System;
using System.Threading.Tasks;

namespace HomeCtl.Host.ProtocolServices
{
	public class NetworkService : Network.NetworkBase
	{
		public override Task<NetworkTimingResponse> GetNetworkTiming(Empty request, ServerCallContext context)
		{
			return Task.FromResult(new NetworkTimingResponse
			{
				ReceivedAtUnixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
			});
		}
	}
}
