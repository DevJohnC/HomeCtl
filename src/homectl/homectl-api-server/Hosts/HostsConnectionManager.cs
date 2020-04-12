using HomeCtl.Connection;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Hosts
{
	/// <summary>
	/// Manages connections to host endpoints.
	/// </summary>
	class HostsConnectionManager
	{
		private readonly ManagedConnections _managedConnections = new ManagedConnections();
		private readonly IConnectionProviderFactory _connectionProviderFactory;

		public HostsConnectionManager(IConnectionProviderFactory connectionProviderFactory)
		{
			_connectionProviderFactory = connectionProviderFactory;
		}

		public async Task Run(CancellationToken stoppingToken)
		{
			var pendingConnectionsTask = _managedConnections.WaitForPendingConnections();
			var runningTasks = new List<Task> { pendingConnectionsTask };

			while (!stoppingToken.IsCancellationRequested)
			{
				var completedTask = await Task.WhenAny(runningTasks);
				runningTasks.Remove(completedTask);

				if (completedTask == pendingConnectionsTask)
				{
					var (pendingConnections, newPendingConnectionsTask) = _managedConnections
						.GetPendingConnectionsAndWaitForMore();

					pendingConnectionsTask = newPendingConnectionsTask;
					runningTasks.Add(pendingConnectionsTask);

					runningTasks.AddRange(pendingConnections.Select(
						connectionManager => connectionManager.Run()
						));
				}
			}
		}

		public void CreateConnectionManager(ManagedHost managedHost)
		{
			var hostConnManager = new ConnectionManager(
				_connectionProviderFactory.CreateConnectionProviders(managedHost.Host));

			_managedConnections.AddPendingConnection(hostConnManager);
		}

		public void UpdateConnectionManager(ManagedHost managedHost)
		{

		}

		private class ManagedConnections
		{
			private readonly object _lockObj = new object();
			private readonly List<ConnectionManager> _pendingConnections = new List<ConnectionManager>();
			private TaskCompletionSource<bool> _pendingConnectionsSignal = new TaskCompletionSource<bool>(false);

			public void AddPendingConnection(ConnectionManager connectionManager)
			{
				lock (_lockObj)
				{
					_pendingConnections.Add(connectionManager);
					if (!_pendingConnectionsSignal.Task.IsCompleted)
						_pendingConnectionsSignal.SetResult(true);
				}
			}

			public Task WaitForPendingConnections()
			{
				lock (_lockObj)
				{
					if (_pendingConnectionsSignal.Task.IsCompleted)
						_pendingConnectionsSignal = new TaskCompletionSource<bool>();
					return _pendingConnectionsSignal.Task;
				}
			}

			public (IEnumerable<ConnectionManager> PendingConnections, Task PendingConnectionsWaitTask) GetPendingConnectionsAndWaitForMore()
			{
				lock (_lockObj)
				{
					var pendingConnections = new List<ConnectionManager>(_pendingConnections);
					_pendingConnections.Clear();
					if (_pendingConnectionsSignal.Task.IsCompleted)
						_pendingConnectionsSignal = new TaskCompletionSource<bool>();
					return (pendingConnections, _pendingConnectionsSignal.Task);
				}
			}
		}
	}
}
