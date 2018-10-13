using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.EmailService;

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorAccountBuilder
    {
        private DominatorAccountModel DominatorAccountModel { get; set; }

        public SocinatorAccountBuilder(string accountId)
        {
            var account = AccountsFileManager.GetAccountById(accountId);
            DominatorAccountModel = account;
        }

        public SocinatorAccountBuilder AddOrUpdateModuleSettings(ActivityType activityType,
            ModuleConfiguration moduleConfiguration)
        {
            var moduleSettings = DominatorAccountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == activityType);

            if (moduleSettings == null)
                DominatorAccountModel.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
            else
            {
                try
                {
                    //if(!moduleConfiguration.IsEnabled)
                    //DominatorScheduler.StopActivity(DominatorAccountModel.AccountBaseModel.AccountId,
                    //           activityType.ToString(), moduleSettings.TemplateId);

                    DominatorAccountModel.ActivityManager.LstModuleConfiguration.Remove(moduleSettings);
                    DominatorAccountModel.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return this;
        }


        public SocinatorAccountBuilder RemoveModuleSettings(ActivityType activityType)
        {
            var moduleSettings = DominatorAccountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == activityType);

            if (moduleSettings != null)
                DominatorAccountModel.ActivityManager.LstModuleConfiguration.Remove(moduleSettings);

            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateCookies(CookieCollection cookies)
        {
            DominatorAccountModel.Cookies = cookies;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDominatorAccountBase(DominatorAccountBaseModel accountBaseModel)
        {
            DominatorAccountModel.AccountBaseModel = accountBaseModel;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateLoginStatus(bool status)
        {
            DominatorAccountModel.IsUserLoggedIn = status;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateUserAgentWeb(string webAgent)
        {
            DominatorAccountModel.UserAgentWeb = webAgent;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateMobileAgentWeb(string webAgent)
        {
            DominatorAccountModel.UserAgentMobile = webAgent;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateMobileRequests(bool isUseOnlyMobileRequest)
        {
            DominatorAccountModel.UseMobileRequestOnly = isUseOnlyMobileRequest;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateExtraParameter(Dictionary<string, string> extraProperity)
        {
            try
            {
                DominatorAccountModel.ExtraParameters = extraProperity;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateExtraParameter(string key, string value)
        {
            try
            {
                if (DominatorAccountModel.ExtraParameters.ContainsKey(key))
                    DominatorAccountModel.ExtraParameters[key] = value;
                else
                    DominatorAccountModel.ExtraParameters.Add(key, value);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn1(int? value)
        {
            DominatorAccountModel.DisplayColumnValue1 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn2(int? value)
        {
            DominatorAccountModel.DisplayColumnValue2 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn3(int? value)
        {
            DominatorAccountModel.DisplayColumnValue3 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn4(int? value)
        {
            DominatorAccountModel.DisplayColumnValue4 = value;
            return this;
        }
        public SocinatorAccountBuilder UpdateLastUpdateTime(int value)
        {
            DominatorAccountModel.LastUpdateTime = value;
            return this;
        }
        public SocinatorAccountBuilder AddOrUpdateProxy(Proxy proxy)
        {
            DominatorAccountModel.AccountBaseModel.AccountProxy = proxy;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateMailCredentials(MailCredentials mailCredentials)
        {
            DominatorAccountModel.MailCredentials = mailCredentials;
            return this;
        }
        public SocinatorAccountBuilder AddOrUpdateIsAutoVerifyByEmail(bool IsAutoVerifyByEmail)
        {
            DominatorAccountModel.IsAutoVerifyByEmail = IsAutoVerifyByEmail;
            return this;
        }
        public SocinatorAccountBuilder AddOrUpdatePaginationId(string key, string value)
        {
            try
            {
                if (DominatorAccountModel.PaginationId.Keys.Contains(key))
                    DominatorAccountModel.PaginationId[key] = value;
                else
                    DominatorAccountModel.PaginationId.Add(key, value);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return this;
        }
        public bool SaveToBinFile()
            => AccountsFileManager.Edit(DominatorAccountModel);
    }
}