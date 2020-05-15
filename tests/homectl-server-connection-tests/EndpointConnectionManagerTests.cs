using HomeCtl.Connection;
using HomeCtl.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
		private static ILoggerFactory _loggerFactory = new LoggerFactory();

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
				new AlwaysYesServerVerifier(), _loggerFactory.CreateLogger<EndpointConnectionManager>()
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
				new AlwaysYesServerVerifier(), _loggerFactory.CreateLogger<EndpointConnectionManager>()
				);
			await endpointConnectionManager.Run(
				new[] { StaticApiServer.AnyOnUri("http://localhost/") },
				new[] { new DisconnectsImmediately() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasLost);
		}

		[TestMethod]
		public async Task Run_Reconnects_Immediately_After_Disconnect()
		{
			var timeout = TimeSpan.FromSeconds(2);
			var grpcHost = await CreateGrpcServer();
			var grpcClient = grpcHost.CreateFixedClient();
			var stopTokenSource = new CancellationTokenSource(timeout);

			var connectionCount = 0;
			var eventBus = new EventBus();
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(args =>
			{
				connectionCount++;
				if (connectionCount == 2)
					stopTokenSource.Cancel();
			});

			var endpointConnectionManager = new EndpointConnectionManager(
				eventBus, new DelegateClientFactory(endpoint => grpcClient),
				new AlwaysYesServerVerifier(), _loggerFactory.CreateLogger<EndpointConnectionManager>()
				);
			await endpointConnectionManager.Run(
				new[] { StaticApiServer.AnyOnUri("http://localhost/") },
				new[] { new DisconnectsImmediately() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.AreEqual(2, connectionCount);
		}

		[TestMethod]
		public async Task Run_Connects_After_Cooldown_If_Connection_Fails()
		{
			var timeout = TimeSpan.FromSeconds(7);
			var grpcHost = await CreateGrpcServer();
			var grpcClient = grpcHost.CreateFixedClient();
			var stopTokenSource = new CancellationTokenSource(timeout);
			var verifier = new VerifiesOnSecondAttemptVerifier();

			var connectionWasEstablished = false;
			var eventBus = new EventBus();
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(args =>
			{
				connectionWasEstablished = true;
				stopTokenSource.Cancel();
			});

			var endpointConnectionManager = new EndpointConnectionManager(
				eventBus, new DelegateClientFactory(endpoint => grpcClient),
				verifier, _loggerFactory.CreateLogger<EndpointConnectionManager>()
				);
			await endpointConnectionManager.Run(
				new[] { StaticApiServer.AnyOnUri("http://localhost/") },
				new[] { new DisconnectsImmediately() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasEstablished);
			//  built-in cooldown is 5 seconds, make the check a little fuzzy
			Assert.IsTrue(verifier.LastAttemptDelta.Value > TimeSpan.FromSeconds(4));
		}

		[TestMethod]
		public async Task Run_Uses_2nd_Endpoint_Immediately_When_Endpoint_Verification_Fails()
		{
			var timeout = TimeSpan.FromSeconds(2);
			var grpcHost = await CreateGrpcServer();
			var grpcClient = grpcHost.CreateFixedClient();
			var stopTokenSource = new CancellationTokenSource(timeout);
			var verifier = new VerifiesOnSecondAttemptVerifier();

			var connectionWasEstablished = false;
			var connectedEndpoint = default(ServerEndpoint);
			var eventBus = new EventBus();
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(args =>
			{
				connectedEndpoint = args.ServerEndpoint;
				connectionWasEstablished = true;
				stopTokenSource.Cancel();
			});

			var endpointConnectionManager = new EndpointConnectionManager(
				eventBus, new DelegateClientFactory(endpoint => grpcClient),
				verifier, _loggerFactory.CreateLogger<EndpointConnectionManager>()
				);
			await endpointConnectionManager.Run(
				new[]
				{
					StaticApiServer.AnyOnUri("http://localhost-first/"),
					StaticApiServer.AnyOnUri("http://localhost-second/")
				},
				new[] { new DisconnectsImmediately() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasEstablished);
			Assert.AreEqual("http://localhost-second/", connectedEndpoint.Uri.OriginalString);
		}

		[TestMethod]
		public async Task Run_Persists_Endpoint_Providers_Across_Failover()
		{
			var timeout = TimeSpan.FromSeconds(2);
			var grpcHost = await CreateGrpcServer();
			var grpcClient = grpcHost.CreateFixedClient();
			var stopTokenSource = new CancellationTokenSource(timeout);
			var verifier = new VerifiesOnSecondAttemptVerifier();

			var connectionWasEstablished = false;
			var connectedEndpoint = default(ServerEndpoint);
			var eventBus = new EventBus();
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(args =>
			{
				connectedEndpoint = args.ServerEndpoint;
				connectionWasEstablished = true;
				stopTokenSource.Cancel();
			});

			var countingEndpointProvider = new CountingProxyEndpointProvider(
				StaticApiServer.AnyOnUri("http://localhost/"));
			var endpointConnectionManager = new EndpointConnectionManager(
				eventBus, new DelegateClientFactory(endpoint => grpcClient),
				verifier, _loggerFactory.CreateLogger<EndpointConnectionManager>()
				);
			await endpointConnectionManager.Run(
				new IServerEndpointProvider[]
				{
					StaticApiServer.AnyOnUri("http://localhost/"),
					countingEndpointProvider
				},
				new[] { new DisconnectsImmediately() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasEstablished);
			Assert.AreEqual(1, countingEndpointProvider.CallCount);
		}

		[TestMethod]
		public async Task Run_Restarts_Endpoint_Provider_After_Throwing_Exception()
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
				new AlwaysYesServerVerifier(), _loggerFactory.CreateLogger<EndpointConnectionManager>()
				);
			await endpointConnectionManager.Run(
				new IServerEndpointProvider[]
				{
					new ThrowsExceptionFirstEndpointProvider(
						StaticApiServer.AnyOnUri("http://localhost/"))
				},
				new[] { new NeverDisconnects() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasEstablished);
		}

		[TestMethod]
		public async Task Run_Uses_2nd_Endpoint_Immediately_When_1st_Throws_Exception()
		{
			var timeout = TimeSpan.FromSeconds(2);
			var grpcHost = await CreateGrpcServer();
			var grpcClient = grpcHost.CreateFixedClient();
			var stopTokenSource = new CancellationTokenSource(timeout);
			var verifier = new VerifiesOnSecondAttemptVerifier();

			var connectionWasEstablished = false;
			var connectedEndpoint = default(ServerEndpoint);
			var eventBus = new EventBus();
			eventBus.Subscribe<EndpointConnectionEvents.Connected>(args =>
			{
				connectedEndpoint = args.ServerEndpoint;
				connectionWasEstablished = true;
				stopTokenSource.Cancel();
			});

			var endpointConnectionManager = new EndpointConnectionManager(
				eventBus, new DelegateClientFactory(endpoint => grpcClient),
				verifier, _loggerFactory.CreateLogger<EndpointConnectionManager>()
				);
			await endpointConnectionManager.Run(
				new IServerEndpointProvider[]
				{
					new ThrowsExceptionFirstEndpointProvider(StaticApiServer.AnyOnUri("http://localhost-first/")),
					StaticApiServer.AnyOnUri("http://localhost-second/")
				},
				new[] { new NeverDisconnects() },
				stopTokenSource.Token
				);
			await grpcHost.StopAsync();

			Assert.IsTrue(connectionWasEstablished);
			Assert.AreEqual("http://localhost-second/", connectedEndpoint.Uri.OriginalString);
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
			public Task<bool> VerifyServer(ServerEndpoint serverEndpoint, HttpClient httpClient, CancellationToken stoppingToken)
			{
				return Task.FromResult(true);
			}
		}

		private class ThrowsExceptionFirstEndpointProvider : IServerEndpointProvider
		{

			private IServerEndpointProvider _proxySink;

			public int CallCount { get; private set; }

			public ThrowsExceptionFirstEndpointProvider(IServerEndpointProvider proxySink)
			{
				_proxySink = proxySink;
			}

			public Task<ServerEndpoint> GetServerEndpoint(CancellationToken stoppingToken)
			{
				CallCount++;
				if (CallCount == 1)
					throw new Exception();
				return _proxySink.GetServerEndpoint(stoppingToken);
			}
		}

		private class CountingProxyEndpointProvider : IServerEndpointProvider
		{
			private IServerEndpointProvider _proxySink;

			public int CallCount { get; private set; }

			public CountingProxyEndpointProvider(IServerEndpointProvider proxySink)
			{
				_proxySink = proxySink;
			}

			public Task<ServerEndpoint> GetServerEndpoint(CancellationToken stoppingToken)
			{
				CallCount++;
				return _proxySink.GetServerEndpoint(stoppingToken);
			}
		}

		private class VerifiesOnSecondAttemptVerifier : IServerIdentityVerifier
		{
			private int _count = 0;

			private DateTime? _lastAttemptTime;

			public TimeSpan? LastAttemptDelta { get; private set; }

			public Task<bool> VerifyServer(ServerEndpoint serverEndpoint, HttpClient httpClient, CancellationToken stoppingToken)
			{
				_count++;
				if (_count == 2)
				{
					LastAttemptDelta = DateTime.Now - _lastAttemptTime.Value;
					_lastAttemptTime = DateTime.Now;
					return Task.FromResult(true);
				}

				_lastAttemptTime = DateTime.Now;
				return Task.FromResult(false);
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
			public Task MonitorForDisconnect(ServerEndpoint serverEndpoint, CancellationToken stoppingToken)
			{
				var tcs = new TaskCompletionSource<bool>();
				stoppingToken.Register(() => tcs.SetResult(true));
				return tcs.Task;
			}
		}

		private class DisconnectsImmediately : IServerLivelinessMonitor
		{
			public Task MonitorForDisconnect(ServerEndpoint serverEndpoint, CancellationToken stoppingToken)
			{
				return Task.CompletedTask;
			}
		}
	}
}
