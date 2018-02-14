using DominatorHouseCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.BusinessLogic
{
    public class DominatorJobProcessFactory : IJobProcessFactory
    {
        static DominatorJobProcessFactory _instance;

        public static DominatorJobProcessFactory Instance => _instance ?? (_instance = new DominatorJobProcessFactory());

        public JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module)
        {
            ActivityType activity = (ActivityType)Enum.Parse(typeof(ActivityType), module);

            switch (activity)
            {
                case ActivityType.Follow:
                    throw new NotImplementedException();

                case ActivityType.Unfollow:
                    throw new NotImplementedException();

                case ActivityType.Like:
                    throw new NotImplementedException();

                case ActivityType.Unlike:
                    throw new NotImplementedException();

                case ActivityType.Comment:
                    throw new NotImplementedException();

                case ActivityType.DeleteComment:
                    throw new NotImplementedException();

                case ActivityType.Post:
                    throw new NotImplementedException();

                case ActivityType.Repost:
                    throw new NotImplementedException();

                case ActivityType.DeletePost:
                    throw new NotImplementedException();

                case ActivityType.Message:
                    throw new NotImplementedException();

                case ActivityType.UserScraper:
                    throw new NotImplementedException();

                case ActivityType.DownloadScraper:
                    throw new NotImplementedException();

                case ActivityType.Reposter:
                    throw new NotImplementedException();

                case ActivityType.Retweet:
                    throw new NotImplementedException();
                    
                default:
                    throw new ArgumentException(module);
            }
        }

        private DominatorJobProcessFactory() { }    // singleton
    }
}
