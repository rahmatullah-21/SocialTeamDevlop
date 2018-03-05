using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.BusinessLogic
{
    /// <summary>
    /// Class which manages all campaigns workflow over the application:
    /// Start, Update, Delete, Duplicate, Pause, Resume, Stop
    /// </summary>
    public class CampaignsWorkflowManager
    {
        CampaignsWorkflowManager _instance = new CampaignsWorkflowManager();
        public CampaignsWorkflowManager Instance => _instance;

        // UI delegate to select accounts
        public Func<List<string>, List<string>> SelectAccountsDialog = selectedAccs =>
        {
            GlobusLogHelper.log.Error("SelectAccountsDialog action handler not set");
            return selectedAccs;
        };


        private CampaignsWorkflowManager() { }


        /// <summary>
        /// Creates and saves new template: ActiviySettings as json, activity type 
        /// </summary>
        /// <param name="activitySettingsJson"></param>
        /// <param name="activityType"></param>
        /// <param name="socialNetworks"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        private TemplateModel CreateTempale(string activitySettingsJson, string activityType, SocialNetworks socialNetworks, string templateName)
        {
            // Initialize and assign the values to TemplateModel for store in bin files
            TemplateModel newTemplate = new TemplateModel
            {
                ActivityType = activityType,
                ActivitySettings = activitySettingsJson,
                CreationDate = DateTime.Now.GetCurrentEpochTime(),
                Name = templateName,
                SocialNetwork = socialNetworks,
            };


            // Serialize the template configuration to bin files
            TemplatesFileManager.Add(newTemplate);

            return newTemplate;            
        }



        /// <summary>
        /// Checks wheter running activities exists for selected users. Asks to overwrite them.
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="selectedAccounts"></param>
        private List<string> CheckExistingActivities(ActivityType activityType, List<string> selectedAccounts)
        {
            Debug.Assert(selectedAccounts.Count > 0);

            var allAccounts = AccountsFileManager.GetAll();

            List<DominatorAccountModel> accountsWithRunningActivity =
                        allAccounts.Where(
                            x => x.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == activityType)
                                                     ?.TemplateId != null)?.ToList() ?? new List<DominatorAccountModel>();

            if (accountsWithRunningActivity.Count == 0)
                return selectedAccounts;

            var selectedAccountsWithRunningActivity = accountsWithRunningActivity.Where(a => selectedAccounts.Contains(a.UserName)).ToList();
            if (selectedAccountsWithRunningActivity.Count == 0)
                return selectedAccounts;

            // Asks for select and overwriting through UI
            selectedAccounts = SelectAccountsDialog(selectedAccountsWithRunningActivity.Select(a => a.UserName).ToList());

            return selectedAccounts;
        }


        /// <summary>
        /// Runs when user clicks Create Campaign
        /// </summary>
        /// <param name="newCampaign"></param>
        public void Create<TModel>(TModel activitySettings, ActivityType activityType, string campaignName, List<string> selectedAccounts)                
        {
            string activitySettingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(activitySettings);
            SocialNetworks socialNetwork = DominatorHouseInitializer.ActiveSocialNetwork;
            TemplateModel template = CreateTempale(activitySettingsJson, activityType.ToString(), socialNetwork, templateName: campaignName);

            // Check existing activities and overwrite them if selected account already has running activity with the same type
            selectedAccounts = CheckExistingActivities(activityType, selectedAccounts);

            // Save 
        }
    }
}
