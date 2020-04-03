using Grpc.Core;
using HomeCtl.Connection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace unittests_homectl_connection
{
	[TestClass]
	public class ConnectionManagerTests
	{
		[TestMethod]
		public async Task Service_Exceptions_Dont_Disconnect_Client()
		{
			using (var testServer = CreateTestServer())
			using (var client = testServer.CreateClient())
			{
				var manager = new ConnectionManager(
					new IConnectionProvider[] { new ConfiguredHttpClientProvider(client) }
					);
				var cancellationTokenSource = new CancellationTokenSource();

				manager.Connected += (s, e) =>
				{
					var grpcClient = new Tests.TestsClient(e.GrpcChannel);
					try
					{
						grpcClient.ThrowAnException(new Empty());
					}
					catch { }
					finally
					{
						Assert.AreEqual(ConnectionManager.ConnectionStates.Connected, manager.ConnectionState);
						cancellationTokenSource.Cancel();
					}
				};

				await manager.Run(cancellationTokenSource.Token);
			}
		}

		[TestMethod]
		public async Task Broken_Connections_Disconnect_Client()
		{
			using (var testServer = CreateTestServer())
			using (var client = testServer.CreateClient())
			{
				var manager = new ConnectionManager(
					new IConnectionProvider[] { new ConfiguredHttpClientProvider(client) }
					);
				var cancellationTokenSource = new CancellationTokenSource();

				manager.Connected += (s, e) =>
				{
					var grpcClient = new Tests.TestsClient(e.GrpcChannel);
					try
					{
						testServer.Dispose();
						grpcClient.DoNothing(new Empty());
					}
					catch { }
					finally
					{
						Assert.AreEqual(ConnectionManager.ConnectionStates.Disconnected, manager.ConnectionState);
						cancellationTokenSource.Cancel();
					}
				};

				await manager.Run(cancellationTokenSource.Token);
			}
		}

		[TestMethod]
		public async Task Will_Reconnect_After_A_Disconnect()
		{
			using (var testServer1 = CreateTestServer())
			using (var client1 = CreateClient(testServer1))
			using (var testServer2 = CreateTestServer())
			using (var client2 = CreateClient(testServer2))
			{
				var connectionProviders = new[]
				{
					new TestHttpClientProvider(client1),
					new TestHttpClientProvider(client2)
				};
				var manager = new ConnectionManager(connectionProviders);
				var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

				var connectedCount = 0;
				var disconnectedCount = 0;

				manager.Connected += (s, e) =>
				{
					connectedCount++;
					var grpcClient = new Tests.TestsClient(e.GrpcChannel);

					if (connectedCount == 1)
					{
						try
						{
							connectionProviders[0].IsDisabled = true;
							testServer1.Dispose();
							grpcClient.DoNothing(new Empty());
						}
						catch { }
					}
					else
					{
						grpcClient.DoNothing(new Empty());
						cancellationTokenSource.Cancel();
					}
				};
				manager.Disconnected += (s, e) =>
				{
					disconnectedCount++;
				};

				await manager.Run(cancellationTokenSource.Token);

				Assert.AreEqual(2, connectedCount);
				Assert.AreEqual(2, disconnectedCount);
			}
		}

		private TestServer CreateTestServer()
		{
			var webHostBuilder = new WebHostBuilder();
			webHostBuilder.ConfigureServices(services => {
				services.AddGrpc();
				});
			webHostBuilder.Configure(builder =>
			{
				builder.UseRouting();

				builder.UseEndpoints(endpoints =>
				{
					endpoints.MapGrpcService<TestServices>();
				});
			});
			return new TestServer(webHostBuilder);
		}

		private HttpClient CreateClient(TestServer testServer)
		{
			AppContext.SetSwitch(
				"System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

			// Need to set the response version to 2.0.
			// Required because of this TestServer issue - https://github.com/aspnet/AspNetCore/issues/16940
			var responseVersionHandler = new ResponseVersionHandler();
			responseVersionHandler.InnerHandler = testServer.CreateHandler();

			var client = new HttpClient(responseVersionHandler);
			client.BaseAddress = new Uri("http://localhost");
			client.DefaultRequestVersion = new Version(2, 0);
			return client;
		}

		private class ResponseVersionHandler : DelegatingHandler
		{
			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				var response = await base.SendAsync(request, cancellationToken);
				response.Version = request.Version;

				return response;
			}
		}

		class TestServices : Tests.TestsBase
		{
			public override Task<Empty> ThrowAnException(Empty request, ServerCallContext context)
			{
				throw new System.Exception("This call throws an exception.");
			}

			public override Task<Empty> DoNothing(Empty request, ServerCallContext context)
			{
				return Task.FromResult(new Empty());
			}
		}

		class TestHttpClientProvider : IConnectionProvider
		{
			private readonly HttpClient _httpClient;

			public bool IsDisabled { get; set; }

			public TestHttpClientProvider(HttpClient httpClient)
			{
				_httpClient = httpClient;
			}

			public Task<ConnectionResult> AttemptConnection(CancellationToken cancellationToken)
			{
				if (IsDisabled)
					return Task.FromResult(ConnectionResult.Failed);
				return Task.FromResult(new ConnectionResult(true, _httpClient));
			}
		}
	}
}
