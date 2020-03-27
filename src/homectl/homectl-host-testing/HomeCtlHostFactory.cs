using System;

namespace homectl
{
	public class HomeCtlHostFactory<TStartup> : IDisposable
		where TStartup : HomeCtlStartup
	{
		public HomeCtlHostFactory()
		{
			_host = BuildHost();
		}

		private readonly HomeCtlHost _host;

		private HomeCtlHost BuildHost()
		{
			return HomeCtlHost.CreateBuilder()
				.UseStartup<TStartup>()
				.Build();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		private void DisposeManagedResources()
		{
			_host.Dispose();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					DisposeManagedResources();
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
