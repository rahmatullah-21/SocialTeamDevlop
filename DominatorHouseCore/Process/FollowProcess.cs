using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Process
{
    internal class FollowProcess : JobProcess
    {
        public FollowProcess()
            : base(dominatorAccountModel: null, jobConfiguration: null, activityType: ActivityType.Follow, currentJobTimeRange: null)
        {
        }

        public FollowProcess(string account, string template, ActivityType activityType, TimingRange CurrentJobTimeRange)
            : base(account, template, activityType, CurrentJobTimeRange)
        {

        }

        public override JobProcess Initialize(string account, string template, ActivityType activity, TimingRange currentJobTimeRange)
        {
            throw new NotImplementedException();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            throw new NotImplementedException();
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            throw new NotImplementedException();
        }

        public override void StartProcess()
        {
            throw new NotImplementedException();
        }
    }
}