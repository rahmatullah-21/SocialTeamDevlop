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
        public static bool SaveProxy(ProxyManagerModel proxy)
        {
            try
            {
                BinFileHelper.SaveProxy(proxy);
                GlobusLogHelper.log.Debug($"Proxy successfully saved");
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }
        public static List<ProxyManagerModel> GetAllProxy()
        {
            return BinFileHelper.GetProxyDetails();
        }
      

        public static void EditProxy(ProxyManagerModel proxy) => BinFileHelper.UpdateProxy(proxy);
        public static void EditAllProxy(List<ProxyManagerModel> proxy) => BinFileHelper.UpdateAllProxy(proxy);


        public static void Delete(Predicate<ProxyManagerModel> match)
        {
            var proxy = BinFileHelper.GetProxyDetails();
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
