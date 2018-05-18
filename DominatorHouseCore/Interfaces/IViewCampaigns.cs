using System.Windows;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    interface IViewCampaigns
    {
        void EditOrDuplicateCampaign(TemplateModel templateModel, CampaignDetails campaignDetails, bool isCampaignEditable, Visibility isCancelEditVisibility, string campaignButtonContent, string templateId);
    }
}