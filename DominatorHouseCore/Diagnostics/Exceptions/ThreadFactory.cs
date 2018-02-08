using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Diagnostics
{
    /// <summary>
    /// Class uses to create tasks with exception handlers
    /// </summary>
    internal class ThreadFactory
    {
        public delegate void TaskError(Task task, Exception error);

        public static readonly ThreadFactory Instance = new ThreadFactory();

        private ThreadFactory() {
            Error += (t, e) =>
            {
                GlobusExceptionHandler.HandleGlobalException(e, t.ToString());                
            };
        }

        private event TaskError Error;

        private void InvokeError(Task task, Exception error)
        {
			Error?.Invoke(task, error);
		}

        public Task Start(Action action)
        {
            var task = new Task(action);
            Start(task);
            return task;
        }

        public Task Start(Action action, TaskCreationOptions options)
        {
            var task = new Task(action, options);
            Start(task);
            return task;
        }

        private void Start(Task task)
        {
            task.ContinueWith(t => InvokeError(t, t.Exception.InnerException),
                                TaskContinuationOptions.OnlyOnFaulted |
                                TaskContinuationOptions.ExecuteSynchronously);
            task.Start();            
        }

        public Task<T> Start<T>(Func<T> action, TaskCreationOptions options)
        {
            var task = new Task<T>(action, options);

            task.ContinueWith(t => InvokeError(t, t.Exception.InnerException),
                                TaskContinuationOptions.OnlyOnFaulted |
                                TaskContinuationOptions.ExecuteSynchronously);
            task.Start();

            return task;
        }
    }
}
