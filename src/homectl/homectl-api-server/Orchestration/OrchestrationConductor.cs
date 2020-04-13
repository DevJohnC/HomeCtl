using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Orchestration
{
	class OrchestrationConductor
	{
		private readonly PendingChangeTracker _changeTracker = new PendingChangeTracker();

		public async Task Run(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var pendingChanges = await _changeTracker.WaitForChanges(stoppingToken);

				if (stoppingToken.IsCancellationRequested)
					break;

				foreach (var change in pendingChanges)
				{
				}
			}
		}

		public void NotifySpecChanged(params object[] changedRecords)
		{
			NotifySpecChanged((IEnumerable<object>)changedRecords);
		}

		public void NotifySpecChanged(IEnumerable<object> changedRecords)
		{
			_changeTracker.AddChangesAndSignal(changedRecords);
		}

		private class PendingChangeTracker
		{
			private readonly object _lockObj = new object();
			private TaskCompletionSource<bool> _changesAddedSignal = new TaskCompletionSource<bool>(false);
			private List<object> _pendingChanges = new List<object>();

			public async Task<IEnumerable<object>> WaitForChanges(CancellationToken stoppingToken)
			{
				var cancellationTaskSource = new TaskCompletionSource<bool>();
				stoppingToken.Register(() => cancellationTaskSource.SetResult(true));

				Task signalTask;
				lock (_lockObj)
				{
					signalTask = _changesAddedSignal.Task;
				}

				await Task.WhenAny(signalTask, cancellationTaskSource.Task);

				lock (_lockObj)
				{
					_changesAddedSignal = new TaskCompletionSource<bool>(false);
					return _pendingChanges.ToArray();
				}
			}

			private void SetWaitingSignalNoLock()
			{
				if (!_changesAddedSignal.Task.IsCompleted)
					_changesAddedSignal.SetResult(true);
			}

			public void AddChangesAndSignal(IEnumerable<object> newChanges)
			{
				lock (_lockObj)
				{
					_pendingChanges.AddRange(newChanges);
					SetWaitingSignalNoLock();
				}
			}
		}
	}
}
