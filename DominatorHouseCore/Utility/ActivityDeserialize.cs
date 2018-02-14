using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityDeserialize
    {
<<<<<<< HEAD
        public static Action<string, string, TimingRange, string> GdScheduler { get; set; }
=======
        /// <summary>
        /// Callback to start scheduler on app startup, create/update campaign, etc.
        /// </summary>
        public static Action<string, string, TimingRange, string> DominatorScheduler { get; set; }
>>>>>>> 90451138ff539015d353c40ecee4b2b96156cd82

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
                case SocialNetworks.Facebook:
                    FdScheduler?.Invoke(AccountUserName, TemplateId, ModuleName);
                    break;
            }
        }

    }
}