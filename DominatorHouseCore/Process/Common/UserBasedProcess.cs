using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;

namespace DominatorHouseCore.Process.Common
{
    public abstract class UserBasedProcess<TUser, TModuleSetting> : JobProcess<TModuleSetting>
        where TUser : class, IInteractedUser
        where TModuleSetting : class
    {
        protected UserBasedProcess(string account, string template, TimingRange currentJobTimeRange) : base(account, template, currentJobTimeRange)
        {
        }


        protected void InitializeFieldsFromDatabaseForInteractedUser()
        {
            try
            {
                var getStartDateofWeek = DateTime.Now.GetStartOfWeek();
                var getTodayDate = DateTime.Today;
                int currentTimeStamp = DateTime.UtcNow.GetCurrentEpochTime();

                var dbContext = DataBaseConnectionAccount.GetContext(DominatorAccountModel.AccountBaseModel.AccountId);

                var dbOperations = new DbOperations(dbContext);


                NoOfActionPerformedCurrentWeek = dbOperations.Count<TUser>(x => (x.ActivityType == ActivityType.ToString()) && (x.InteractionDateTime >= getStartDateofWeek));
                NoOfActionPerformedCurrentDay = dbOperations.Count<TUser>(x => (x.ActivityType == ActivityType.ToString()) && x.InteractionDateTime >= getTodayDate);
                NoOfActionPerformedCurrentHour = dbOperations.Count<TUser>(x => (x.ActivityType == ActivityType.ToString()) && currentTimeStamp - x.InteractionTimeStamp <= 3600);
            }
            catch (Exception ex)
            {
                ex.DebugLog("Error in Scheduling.");
            }
        }
    }
}
