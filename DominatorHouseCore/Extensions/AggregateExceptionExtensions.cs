using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Extensions
{
    public static class AggregateExceptionExtensions
    {
        public static void HandleOperationCancellation(this AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
            {
                if (e is TaskCanceledException || e is OperationCanceledException)
                    throw new OperationCanceledException(@"Cancellation Requested !", e);
                else
                    e.DebugLog(e.StackTrace + e.Message);
            }
        }
    }
}
