using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Utility
{
    public class ActivityDeserialize
    {

        public static Action<string, string, TimingRange, string> GdScheduler { get; set; }

        public static Action<string, string, string> FdScheduler { get; set; }

        public string ModuleName { get; set; }

        public string TemplateId { get; set; }

        public string AccountUserName { get; set; }

        public SocialNetworks SocialNetworks { get; set; }

        public ActivityDeserialize(string moduleName, string templateId, string accountUserName,TimingRange timing, SocialNetworks socialNetworks)
        {
            ModuleName = moduleName;
            TemplateId = templateId;
            AccountUserName = accountUserName;
            SocialNetworks = socialNetworks;

            switch (socialNetworks)
            {
                case SocialNetworks.Instagram:
                    ActivityDeserialize.GdScheduler(AccountUserName, TemplateId, timing, ModuleName);
                    break;
                case SocialNetworks.Facebook:
                    ActivityDeserialize.FdScheduler(AccountUserName, TemplateId, ModuleName);
                    break;
            }
        }

    }
}