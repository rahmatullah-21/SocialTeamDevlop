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
                GlobusLogHelper.log.Debug($"Process stopped [{key}]");
                //  DictAllJobCancellationTokenSources.Remove(key);
                DictAllJobCancellationTokenSources[key] = DominatorCancellationTokenSource;
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }

        }


        public static bool StopTask(string userName,string templateId)
        {
            try
            {
               return StopTask(userName + "_" + templateId);
          
            }
            catch (Exception Ex)
            {
                return false;
            }

        }

        public static bool IsStarted(string userName, string templateId)
        {
            return DictAllJobCancellationTokenSources.ContainsKey($"{userName}_{templateId}");
        }
    }


    public class DominatorCancellationTokenSource : CancellationTokenSource
    {
        public static object syncDict = new object();

        public DominatorCancellationTokenSource(string userName , string templateId) 
        {
            lock (syncDict)
            {
                string key = userName + "_" + templateId;

                if (!TaskAndThreadUtility.DictAllJobCancellationTokenSources.ContainsKey(key))
                    TaskAndThreadUtility.DictAllJobCancellationTokenSources.Add(key, this);
            }
        }        
    }




}
