using DominatorHouseCore.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.ViewModel.Common
{
    public abstract class SynchronizedViewModel : BindableBase
    {
        private readonly SemaphoreSlim _asyncSyncContext = new SemaphoreSlim(1, 1);
        private readonly object _syncContext = new object();
        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                SetProperty(ref _isRunning, value);
                OnPropertyChanged(nameof(IsIdle));
            }
        }

        public bool IsIdle
        {
            get { return !_isRunning; }
        }

        public async Task ExecuteSynchronized<T1>(Func<T1, Task> asyncOperation, T1 input1)
        {
            await _asyncSyncContext.WaitAsync();
            try
            {
                IsRunning = true;
                await asyncOperation(input1);
            }
            finally
            {
                IsRunning = false;
                _asyncSyncContext.Release();
            }
        }

        public async Task ExecuteSynchronized<T1, T2>(Func<T1, T2, Task> asyncOperation, T1 input1, T2 input2)
        {
            await _asyncSyncContext.WaitAsync();
            try
            {
                IsRunning = true;
                await asyncOperation(input1, input2);
            }
            finally
            {
                IsRunning = false;
                _asyncSyncContext.Release();
            }
        }

        public void ExecuteSynchronized(Action action)
        {
            lock (_syncContext)
            {
                try
                {
                    IsRunning = true;
                    action();
                }
                finally
                {
                    IsRunning = false;
                }
            }
        }
    }
}
