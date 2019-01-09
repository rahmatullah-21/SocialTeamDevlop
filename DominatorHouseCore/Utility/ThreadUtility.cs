using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public interface IThreadUtility
    {
        void Sleep(int timeout);
        void Sleep(TimeSpan timeout);
        Task Delay(int timeout, CancellationToken token);
    }

    [ExcludeFromCodeCoverage]
    public class ThreadUtility : IThreadUtility
    {

        public void Sleep(int timeout)
        {
            Thread.Sleep(timeout);
        }
        public void Sleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
        }

        public Task Delay(int timeout, CancellationToken token)
        {
            return Task.Delay(timeout, token);
        }

    }
}