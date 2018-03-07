using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public static class ProxyFileManager
    {
        public static bool SaveProxy<T>(T proxy) where T : class
        {
            try
            {
                BinFileHelper.SaveProxy(proxy);
                GlobusLogHelper.log.Debug($"Proxy successfully saved");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        public static List<ProxyManagerModel> GetAllProxy()
        {
            return BinFileHelper.GetProxyDetails<ProxyManagerModel>();
        }
      

        public static void EditProxy<TModel>(TModel proxy) where TModel : class => BinFileHelper.UpdateProxy(proxy);

        public static void Delete<PModel>(Predicate<PModel> match) where PModel : class
        {
            var proxy = BinFileHelper.GetProxyDetails<PModel>();
            var toDelete = proxy.FindAll(match);
            proxy.RemoveAll(match);
            BinFileHelper.UpdateAllProxy(proxy);
        }
       
        public static ProxyManagerModel GetProxyByName(string ProxyName)
        {
           return GetAllProxy().FirstOrDefault(x => x.AccountProxy.ProxyName == ProxyName);
        }


    }
}
