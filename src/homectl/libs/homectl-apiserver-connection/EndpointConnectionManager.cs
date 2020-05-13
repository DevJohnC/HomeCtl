﻿using Grpc.Core;
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
		public ServerEndpoint Endpoint { get; private set; }
		public ChannelBase? ServicesChannel { get; private set; }

		public EndpointConnectionManager(EventBus eventBus,
			IEndpointClientFactory endpointClientFactory,
			IServerIdentityVerifier serverIdentifyVerifier)
		{
			EventBus = eventBus;
			_endpointClientFactory = endpointClientFactory;
			_serverIdentifyVerifier = serverIdentifyVerifier;
		}

		private void SetDisconnectedState()
		{
			var previousStatus = ConnectionStatus;
			ConnectionStatus = ConnectionStatus.NotConnected;

			if (previousStatus == ConnectionStatus.Connected)
			{
				EventBus.Publish(
					new EndpointConnectionEvents.Disconnected(this, Endpoint)
					);
			}
		}

		private void SetConnectedState()
		{
			var previousStatus = ConnectionStatus;
			ConnectionStatus = ConnectionStatus.Connected;

			if (previousStatus == ConnectionStatus.NotConnected)
			{
				EventBus.Publish(
					new EndpointConnectionEvents.Connected(this, Endpoint)
					);
			}
		}

		public async Task Run(IEnumerable<IServerEndpointProvider> endpointProviders,
			IEnumerable<IServerLivelinessMonitor> livelinessMonitors,
			CancellationToken stoppingToken)
		{
			var endpointProviderArray = endpointProviders?.ToArray();
			var livelinessMonitorsArray = livelinessMonitors?.ToArray();
			if (endpointProviderArray == null || endpointProviderArray.Length == 0)
				throw new Exception("Impossible to locate server, provide 1 or more endpoint providers.");
			if (livelinessMonitorsArray == null || livelinessMonitorsArray.Length == 0)
				throw new Exception("Impossible to monitor server connection, provide 1 or more liveliness monitors.");

			while (!stoppingToken.IsCancellationRequested)
			{
				switch (ConnectionStatus)
				{
					case ConnectionStatus.NotConnected:
						try
						{
							await ConnectToServer(endpointProviderArray, stoppingToken);
						}
						catch (Exception ex) //  keep on attempting to connect despite exceptions
						{
							//  todo: log exceptions
						}

						if (!stoppingToken.IsCancellationRequested &&
							ConnectionStatus != ConnectionStatus.Connected)
						{
							//  wait a cooldown before attempting to reconnect
							try
							{
								await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
							}
							catch { }
						}
						break;
					case ConnectionStatus.Connected:
						try
						{
							await WaitForDisconnect(livelinessMonitorsArray, stoppingToken);
						}
						catch (Exception ex)
						{
							//  todo: log exceptions
							SetDisconnectedState();
						}
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

			try
			{
				while (!stoppingToken.IsCancellationRequested && endpointResolveTasks.Count > 0 &&
					ConnectionStatus != ConnectionStatus.Connected)
				{
					var completedTask = await Task.WhenAny(endpointResolveTasks);
					endpointResolveTasks.Remove(completedTask);

					try
					{
						var endpoint = await completedTask;

						if (stoppingToken.IsCancellationRequested)
							return;

						var httpClient = _endpointClientFactory.CreateHttpClient(endpoint);

						if (!(await _serverIdentifyVerifier.VerifyServer(endpoint, httpClient)))
							continue;

						Endpoint = endpoint;
						ServicesChannel = Grpc.Net.Client.GrpcChannel.ForAddress(
							endpoint.Uri, new Grpc.Net.Client.GrpcChannelOptions
							{
								DisposeHttpClient = false,
								HttpClient = httpClient
							});

						SetConnectedState();
					}
					catch (Exception ex)
					{
						//  todo: log exception
					}
				}
			}
			finally
			{
				stopConnectingToken.Cancel();
			}
		}

		private async Task WaitForDisconnect(IServerLivelinessMonitor[] livelinessMonitors, CancellationToken stoppingToken)
		{
			var cancellationSource = new CancellationTokenSource();
			var combinedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
				stoppingToken, cancellationSource.Token
				);

			try
			{
				var monitoringTasks = livelinessMonitors
					.Select(q => q.MonitorForDisconnect(combinedCancellationSource.Token))
					.ToArray();

				var finishedTask = await Task.WhenAny(monitoringTasks);
				//  await the finished task to observe any exceptions
				await finishedTask;

				if (stoppingToken.IsCancellationRequested)
					return;

				SetDisconnectedState();
			}
			catch (Exception ex)
			{
				//  todo: log exception
				throw;
			}
			finally
			{
				cancellationSource.Cancel();
				ConnectionStatus = ConnectionStatus.NotConnected;
			}
		}
	}

	public static class EndpointConnectionEvents
	{
		public class Connected
		{
			public Connected(EndpointConnectionManager endpointConnectionManager,
				ServerEndpoint serverEndpoint)
			{
				EndpointConnectionManager = endpointConnectionManager;
				ServerEndpoint = serverEndpoint;
			}

			public EndpointConnectionManager EndpointConnectionManager { get; }
			public ServerEndpoint ServerEndpoint { get; }
		}

		public class Disconnected
		{
			public Disconnected(EndpointConnectionManager endpointConnectionManager,
				ServerEndpoint serverEndpoint)
			{
				EndpointConnectionManager = endpointConnectionManager;
				ServerEndpoint = serverEndpoint;
			}

			public EndpointConnectionManager EndpointConnectionManager { get; }
			public ServerEndpoint ServerEndpoint { get; }
		}
	}
}
