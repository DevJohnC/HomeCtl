using HomeCtl.ApiServer;
using HomeCtl.Connection;
using HomeCtl.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace homectl_server_connection_tests
{
	[TestClass]
	public class EndpointConnectionManagerTests
	{
		[TestMethod]
		public async Task Run_Connects_To_Endpoint()
		{
			var timeout = TimeSpan.FromSeconds(2);
			var grpcHost = await CreateGrpcServer();
			var grpcClient = grpcHost.CreateFixedClient();
			var stopTokenSource = new CancellationTokenSource(timeout);

			var connectionWasEstablished = false;
			var eventBus = new EventBus();
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(args =>
			{
				connectionWasEstablished = true;
				stopTokenSource.Cancel();
			});

			var endpointConnectionManager = new EndpointConnectionManager(
				eventBus, new DelegateClientFactory(endpoint => grpcClient),
				new AlwaysYesServerVerifier()
				);
			await endpointConnectionManager.Run(
				new[] { StaticApiServer.AnyOnUri("http://localhost/") },
				new[] { new NeverDisconnects() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasEstablished);
		}

		[TestMethod]
		public async Task Run_Disconnects_When_Monitor_Returns()
		{
			var timeout = TimeSpan.FromSeconds(2);
			var grpcHost = await CreateGrpcServer();
			var grpcClient = grpcHost.CreateFixedClient();
			var stopTokenSource = new CancellationTokenSource(timeout);

			var connectionWasLost = false;
			var eventBus = new EventBus();
			eventBus.Subscribe<EndpointConnectionEvents.Disconnected>(args =>
			{
				connectionWasLost = true;
				stopTokenSource.Cancel();
			});

			var endpointConnectionManager = new EndpointConnectionManager(
				eventBus, new DelegateClientFactory(endpoint => grpcClient),
				new AlwaysYesServerVerifier()
				);
			await endpointConnectionManager.Run(
				new[] { StaticApiServer.AnyOnUri("http://localhost/") },
				new[] { new DisconnectsImmediately() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasLost);
		}

		private async Task<IHost> CreateGrpcServer()
		{
			var builder = new HostBuilder()
				.ConfigureWebHost(builder => builder
					.UseStartup<GrpcServerStartup>().UseTestServer());
			return await builder.StartAsync();
		}

		private class AlwaysYesServerVerifier : IServerIdentityVerifier
		{
			public Task<bool> VerifyServer(ServerEndpoint serverEndpoint, HttpClient httpClient)
			{
				return Task.FromResult(true);
			}
		}

		private class GrpcServerStartup
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddGrpc();
			}

			public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
			{
				app.UseRouting();
				app.UseEndpoints(routes =>
				{
				});
			}
		}

		private class DelegateClientFactory : IEndpointClientFactory
		{
			private Func<ServerEndpoint, HttpClient> _httpClientFactory;

			public DelegateClientFactory(Func<ServerEndpoint, HttpClient> httpClientFactory)
			{
				_httpClientFactory = httpClientFactory;
			}

			public HttpClient CreateHttpClient(ServerEndpoint serverEndpoint)
			{
				return _httpClientFactory(serverEndpoint);
			}
		}

		private class NeverDisconnects : IServerLivelinessMonitor
		{
			public Task MonitorForDisconnect(CancellationToken stoppingToken)
			{
				var tcs = new TaskCompletionSource<bool>();
				stoppingToken.Register(() => tcs.SetResult(true));
				return tcs.Task;
			}
		}

		private class DisconnectsImmediately : IServerLivelinessMonitor
		{
			public Task MonitorForDisconnect(CancellationToken stoppingToken)
			{
				return Task.CompletedTask;
			}
		}
	}
}
