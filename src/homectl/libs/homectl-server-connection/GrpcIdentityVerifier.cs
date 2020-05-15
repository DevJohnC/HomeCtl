using Grpc.Core.Logging;
using Grpc.Net.Client;
using HomeCtl.Services.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public class GrpcIdentityVerifier : IServerIdentityVerifier
	{
		private readonly ILogger<GrpcIdentityVerifier> _logger;

		public GrpcIdentityVerifier(ILogger<GrpcIdentityVerifier> logger)
		{
			_logger = logger;
		}

		public async Task<bool> VerifyServer(ServerEndpoint serverEndpoint, HttpClient httpClient, CancellationToken stoppingToken)
		{
			var channel = GrpcChannel.ForAddress(serverEndpoint.Uri, new GrpcChannelOptions
			{
				DisposeHttpClient = false,
				HttpClient = httpClient
			});

			var client = new Information.InformationClient(channel);

			try
			{
				var version = await client.GetServerVersionAsync(Services.Empty.Instance);
			}
			catch
			{
				_logger.LogDebug($"Server couldn't be reached");
				return false;
			}

			_logger.LogDebug($"Contacted server");

			//  todo: query server for identity credentials
			return serverEndpoint.IdentityPolicy.IsValid(
				default
				);
		}
	}
}
