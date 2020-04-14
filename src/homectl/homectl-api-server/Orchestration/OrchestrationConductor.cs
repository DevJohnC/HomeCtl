using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Orchestration
{
	class OrchestrationConductor
	{
		private readonly PendingChangeTracker _changeTracker = new PendingChangeTracker();
		private readonly ISpecApplierFactory _applierFactory;

		public OrchestrationConductor(ISpecApplierFactory applierFactory)
		{
			_applierFactory = applierFactory;
		}

		public async Task Run(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var pendingPatches = await _changeTracker.WaitForChanges(stoppingToken);

				if (stoppingToken.IsCancellationRequested)
					break;

				foreach (var filteredPatch in pendingPatches)
				{
					await ApplyChange(filteredPatch);
				}
			}
		}

		private async Task ApplyChange(FilteredSpecPatch specPatch)
		{
			var applier = _applierFactory.GetApplier();
			var applyResult = await applier.ApplySpecChanges(specPatch);
		}

		public void EnqueueSpecPatch(params FilteredSpecPatch[] changedRecords)
		{
			EnqueueSpecPatch((IEnumerable<FilteredSpecPatch>)changedRecords);
		}

		public void EnqueueSpecPatch(IEnumerable<FilteredSpecPatch> changedRecords)
		{
			_changeTracker.AddChangesAndSignal(changedRecords);
		}

		private class PendingChangeTracker
		{
			private readonly object _lockObj = new object();
			private TaskCompletionSource<bool> _changesAddedSignal = new TaskCompletionSource<bool>(false);
			private List<FilteredSpecPatch> _pendingPatches = new List<FilteredSpecPatch>();

			public async Task<IEnumerable<FilteredSpecPatch>> WaitForChanges(CancellationToken stoppingToken)
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
					return _pendingPatches.ToArray();
				}
			}

			private void SetWaitingSignalNoLock()
			{
				if (!_changesAddedSignal.Task.IsCompleted)
					_changesAddedSignal.SetResult(true);
			}

			public void AddChangesAndSignal(IEnumerable<FilteredSpecPatch> newChanges)
			{
				lock (_lockObj)
				{
					_pendingPatches.AddRange(newChanges);
					SetWaitingSignalNoLock();
				}
			}
		}
	}
}
