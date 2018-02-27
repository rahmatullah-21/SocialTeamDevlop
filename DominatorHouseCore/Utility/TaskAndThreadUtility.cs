using DominatorHouseCore.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public static class TaskAndThreadUtility
    {
        public static Dictionary<string, DominatorCancellationTokenSource> DictAllJobCancellationTokenSources =
            new Dictionary<string, DominatorCancellationTokenSource>();

        public static bool IsCancelled(string key)
        {
            if (!DictAllJobCancellationTokenSources.Keys.Contains(key))
                return false;
            return DictAllJobCancellationTokenSources[key].IsCancellationRequested;
        }

        private static bool StopTask(string key)
        {
            try
            {
                if (!DictAllJobCancellationTokenSources.Keys.Contains(key))
                    return false;
                DominatorCancellationTokenSource DominatorCancellationTokenSource = DictAllJobCancellationTokenSources[key];
                DominatorCancellationTokenSource.Cancel();
                GlobusLogHelper.log.Info("Process stopped");
                //  DictAllJobCancellationTokenSources.Remove(key);
                DictAllJobCancellationTokenSources[key] = DominatorCancellationTokenSource;
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }

        }


        public static bool StopTask(string accountId,string templateId)
        {
            try
            {
               return StopTask(accountId + "_" + templateId);
          
            }
            catch (Exception Ex)
            {
                return false;
            }

        }








    }

    public class DominatorCancellationTokenSource : CancellationTokenSource
    {
        public DominatorCancellationTokenSource(string accountId , string templateId) : this(accountId + "_" + templateId)
        {
        }

        public DominatorCancellationTokenSource(string key) :base()
        {
            if (TaskAndThreadUtility.DictAllJobCancellationTokenSources.ContainsKey(key))
                TaskAndThreadUtility.DictAllJobCancellationTokenSources.Remove(key);
            TaskAndThreadUtility.DictAllJobCancellationTokenSources.Add(key, this);
        }
    }




}
