using HomeCtl.Connection;
using HomeCtl.Host;
using HomeCtl.Kinds;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddApiServer(this IServiceCollection services, IServerEndpointProvider serverConnector)
		{
			services.AddSingleton(serverConnector);
			return services;
		}

		public static IServiceCollection AddApiServer<T>(this IServiceCollection services)
			where T : class, IServerEndpointProvider
		{
			services.AddSingleton<IServerEndpointProvider, T>();
			return services;
		}

		public static IServiceCollection AddKind(this IServiceCollection services, Kind kind)
		{
			services.Configure<HostOptions>(options =>
			{
				options.Kinds.Add(kind);
			});
			return services;
		}

		public static IServiceCollection AddDeviceProvider<TDeviceProvider>(this IServiceCollection services)
			where TDeviceProvider : class, IDeviceProvider
		{
			services.AddSingleton<IDeviceProvider, TDeviceProvider>(sP =>
				sP.GetRequiredService<TDeviceProvider>());
			services.AddSingleton<TDeviceProvider>();
			return services;
		}

		public static IServiceCollection AddDeviceProvider<TDeviceProvider>(this IServiceCollection services, TDeviceProvider deviceProvider)
			where TDeviceProvider : class, IDeviceProvider
		{
			services.AddSingleton<IDeviceProvider>(deviceProvider);
			services.AddSingleton<TDeviceProvider>(deviceProvider);
			return services;
		}
	}
}
