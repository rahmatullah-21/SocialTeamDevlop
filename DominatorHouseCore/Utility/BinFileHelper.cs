using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.IO;

namespace DominatorHouseCore.Utility
{
    //public class BinFileHelpe
    //{

    //    #region Read Accounts

    //    private static readonly object AccountReadLocker = new object();

    //    public static List<AccountModel> ReadAccounts(SocialNetworks socialNetwork)
    //    {
    //        lock (AccountReadLocker)
    //        {
    //            return ProtoBuffBase.DeserializeObjects<AccountModel>(ConstantVariable.GetIndexAccountPath(socialNetwork) + $"//{ConstantVariable.AccountDetails}");
    //        }
    //    }

    //    #endregion

    //    public static ObservableCollectionBase<string> GetUsers(SocialNetworks socialNetwork)
    //    {
    //        lock (AccountReadLocker)
    //            return new ObservableCollectionBase<string>(ReadAccounts(socialNetwork).Select(x => x.UserName).ToList());
    //    }

    //    public static List<AccountModel> GetAccountDetail(SocialNetworks socialNetwork)
    //    {
    //        lock (AccountReadLocker)
    //            return ProtoBuffBase.DeserializeObjects<AccountModel>(
    //                   ConstantVariable.GetIndexAccountPath(socialNetwork) + $"//{ConstantVariable.AccountDetails}");
    //    }


    //    public static bool UpdateAccountDetail(List<AccountModel> lstAccountModel , SocialNetworks socialNetworks)
    //    {
    //        lock (AccountReadLocker)
    //            return ProtoBuffBase.SerializeListObject<AccountModel>(lstAccountModel,
    //                ConstantVariable.GetIndexAccountPath(socialNetworks) + $"//{ConstantVariable.AccountDetails}");
    //    }


    //    public static void DeleteAccountDetailBinFile()
    //    {
    //        lock (AccountReadLocker)
    //            File.Delete(ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram) + $"//{ConstantVariable.AccountDetails}");
    //    }

    //    public static List<CampaignDetails> GetCampaignDetail()
    //    {
    //        lock (AccountReadLocker)
    //            return ProtoBuffBase.DeserializeObjects<CampaignDetails>(
    //            $"{ConstantVariable.socialNetworkPath(SocialNetworks.Instagram)}//{ConstantVariable.CampaignDetails}"); //ConstantVariable.socialNetworkPath(SocialNetworks.Instagram)
    //    }

    //    public static void DeleteCampaignDetailsBinFile()
    //    {
    //        lock (AccountReadLocker)
    //            File.Delete($"{ConstantVariable.socialNetworkPath(SocialNetworks.Instagram)}//{ConstantVariable.CampaignDetails}");
    //    }

    //    public static List<TemplateModel> GetTemplateDetails()
    //    {
    //        lock (AccountReadLocker)
    //            return ProtoBuffBase.DeserializeObjects<TemplateModel>(
    //            $"{ConstantVariable.socialConfigurationPath(SocialNetworks.Instagram)}//{ConstantVariable.TemplateBinName}");//ConstantVariable.socialConfigurationPath(SocialNetworks.Instagram)
    //    }

    //    public static List<T> GetBinFileDetails<T>() where T : class
    //    {
    //        lock (AccountReadLocker)
    //            return ProtoBuffBase.DeserializeObjects<T>(GetBinFilePath<T>());//ConstantVariable.socialConfigurationPath(SocialNetworks.Instagram)
    //    }



    //    public static void DeleteTemplateDetailsBinFile()
    //    {
    //        lock (AccountReadLocker)
    //            File.Delete($"{ConstantVariable.socialConfigurationPath(SocialNetworks.Instagram)}//{ConstantVariable.TemplateBinName}");
    //    }

    //    public static void DeleteBinFile<T>()
    //    {
    //        lock (AccountReadLocker)
    //            File.Delete(GetBinFilePath<T>());
    //    }


    //    private static object updateAccountLocker = new object();

    //    /// <summary>
    //    /// update perticularr accountmodel to the account bin file
    //    /// </summary>
    //    /// <param name="accountModel"></param>
    //    /// <returns></returns>
    //    public static bool UpdateAccount(AccountModel accountModel)
    //    {
    //        bool isUpdated;
    //        try
    //        {
    //            lock (updateAccountLocker)
    //            {
    //                List<AccountModel> lstAllAccountModel = GetAccountDetail();
    //                AccountModel accountModelProto = lstAllAccountModel.FirstOrDefault(x => x.AccountId == accountModel.AccountId);
    //                lstAllAccountModel[lstAllAccountModel.IndexOf(accountModelProto)] = accountModel;

    //                isUpdated = UpdateAccountDetail(lstAllAccountModel);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e);
    //            throw;
    //        }

    //        return isUpdated;
    //    }


    //    public static bool UpdateBinFile<T>(List<T> lstTemplateModels) where T : class
    //    {
    //        try
    //        {
    //            DeleteBinFile<T>();
    //            ProtoBuffBase.SerializeListObject<T>(lstTemplateModels, GetBinFilePath<T>());
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {

    //            return false;
    //        }

    //    }

    //    private static string GetBinFilePath<T>()
    //    {
    //        string filePath = String.Empty;
    //        if (typeof(T) == typeof(TemplateModel))
    //            filePath =
    //                $"{ConstantVariable.socialConfigurationPath(SocialNetworks.Instagram)}//{ConstantVariable.TemplateBinName}";
    //        else if (typeof(T) == typeof(CampaignDetails))
    //            filePath =
    //                $"{ConstantVariable.socialNetworkPath(SocialNetworks.Instagram)}//{ConstantVariable.CampaignDetails}";
    //        else if (typeof(T) == typeof(AccountModel))
    //            filePath = ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram) +
    //                       $"//{ConstantVariable.AccountDetails}";
    //        return filePath;
    //    }

    //}
}
