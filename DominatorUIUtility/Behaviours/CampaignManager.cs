using System;
using System.Windows;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.Behaviours
{
    public static class CampaignManager
    {
        public static Action<TemplateModel, CampaignDetails, bool, Visibility, string, string> EditOrDuplicateCampaign { get; set; }

    }
}