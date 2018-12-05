using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Utility
{
    public static class ListHelper
    {
        /// <summary>
        /// Shuffles items of a given list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int index = 0; index < list.Count; ++index)
                list.Swap(index, RandomUtilties.GetRandomNumber(list.Count - 1, index));
        }
        
        /// <summary>
        /// Swaps two given integet values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            T obj = list[i];
            list[i] = list[j];
            list[j] = obj;
        }
        public static void ShuffleAccountsForAds(this IList<DominatorAccountModel> list)
        {
            for (int index = 0; index < list.Count; ++index)
                list.Swap(index, RandomUtilties.GetRandomNumber(list.Count - 1, index));

            List<DominatorAccountModel> lstAccount = new List<DominatorAccountModel>();

            int fbStartCount = 0;
            int gdStartCount = 0;
            int rdStartCount = 0;

            while (lstAccount.Count < list.Count)
            {
                var facebookAccountList = list.Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Skip(fbStartCount).Take(15).ToList();
                var instagramAccountList = list.Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram).Skip(gdStartCount).Take(15).ToList();
                var redditAccountList = list.Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Reddit).Skip(rdStartCount).Take(5).ToList();

                fbStartCount += 15;
                gdStartCount += 15;
                rdStartCount += 5;

                lstAccount.AddRange(facebookAccountList);
                lstAccount.AddRange(instagramAccountList);
                lstAccount.AddRange(redditAccountList);
            }

            // ReSharper disable once RedundantAssignment
            list = lstAccount;

        }
    }
}
