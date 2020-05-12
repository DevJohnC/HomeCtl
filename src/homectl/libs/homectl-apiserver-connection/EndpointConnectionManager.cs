using Grpc.Core;
using HomeCtl.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public class EndpointConnectionManager
	{
		protected readonly EventBus EventBus;
		private readonly IEndpointClientFactory _endpointClientFactory;
		private readonly IServerIdentityVerifier _serverIdentifyVerifier;

		public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.NotConnected;

		public ChannelBase? ServicesChannel { get; private set; }

		public EndpointConnectionManager(EventBus eventBus,
			IEndpointClientFactory endpointClientFactory,
			IServerIdentityVerifier serverIdentifyVerifier)
		{
			EventBus = eventBus;
			_endpointClientFactory = endpointClientFactory;
			_serverIdentifyVerifier = serverIdentifyVerifier;
		}

		public async Task Run(IEnumerable<IServerEndpointProvider> endpointProviders, CancellationToken stoppingToken)
		{
			var endpointProviderArray = endpointProviders.ToArray();
			if (endpointProviderArray.Length == 0)
				throw new Exception("Impossible to locate server, provide 1 or more endpoint providers.");

			while (!stoppingToken.IsCancellationRequested)
			{
				switch (ConnectionStatus)
				{
					case ConnectionStatus.NotConnected:
						await ConnectToServer(endpointProviderArray, stoppingToken);
						EventBus.Publish(
							new EndpointConnectionEvents.Connected(this)
							);
						break;
					case ConnectionStatus.Connected:
						await WaitForDisconnect(stoppingToken);
						break;
				}
			}
		}

		private async Task ConnectToServer(IServerEndpointProvider[] endpointProviders, CancellationToken stoppingToken)
		{
			//  run all the endpoint providers simultaneously
			//  attempt to connect to endpoints as they are available
			var stopConnectingToken = new CancellationTokenSource();
			var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
				stoppingToken, stopConnectingToken.Token
				);

			var endpointResolveTasks = endpointProviders.Select(q => q.GetServerEndpoint(cancellationSource.Token))
				.ToList();

			while (!stoppingToken.IsCancellationRequested && endpointResolveTasks.Count > 0)
			{
				var completedTask = await Task.WhenAny(endpointResolveTasks);
				endpointResolveTasks.Remove(completedTask);

				try
				{
					var endpoint = await completedTask;

					var httpClient = _endpointClientFactory.CreateHttpClient(endpoint);

					if (!(await _serverIdentifyVerifier.VerifyServer(endpoint, httpClient)))
						continue;

					ServicesChannel = Grpc.Net.Client.GrpcChannel.ForAddress(
						endpoint.Uri, new Grpc.Net.Client.GrpcChannelOptions
						{
							DisposeHttpClient = false,
							HttpClient = httpClient
						});

					ConnectionStatus = ConnectionStatus.Connected;
					return;
				}
				catch (Exception ex)
				{
					//  todo: log exception
				}
			}
		}

		private Task WaitForDisconnect(CancellationToken stoppingToken)
		{
			var tcs = new TaskCompletionSource<bool>();
			stoppingToken.Register(() => tcs.SetCanceled());
			return tcs.Task;
		}
	}

	public static class EndpointConnectionEvents
	{
		public class Connected
		{
			public Connected(EndpointConnectionManager endpointConnectionManager)
			{
				EndpointConnectionManager = endpointConnectionManager;
			}

			public EndpointConnectionManager EndpointConnectionManager { get; }
		}
	}
}
