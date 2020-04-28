using HomeCtl.Host;
using HomeCtl.Kinds;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceExtensions
	{
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
