using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Process.Common
{
    public abstract class FollowProcess<TUser, TModuleSetting> : UserBasedProcess<TUser, TModuleSetting>
        where TUser : class, IInteractedUser
        where TModuleSetting : class
    {
        public override ActivityType ActivityType => ActivityType.Follow;

        protected FollowProcess(string account, string template, TimingRange currentJobTimeRange) : base(account, template, currentJobTimeRange)
        {
            InitializeFieldsFromDatabaseForInteractedUser();
        }
    }
}
