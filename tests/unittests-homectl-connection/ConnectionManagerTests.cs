using Grpc.Core;
using HomeCtl.Connection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

		class TestServices : Tests.TestsBase
		{
			public override Task<Empty> ThrowAnException(Empty request, ServerCallContext context)
			{
				throw new System.Exception("This call throws an exception.");
			}
		}
	}
}
