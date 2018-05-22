using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.Views.SocioPublisher
{
    public class PublisherViewUtility
    {
        public void OpenPostlistSettings(string campaignId)
        {
            var publisherPostlistSettingsModel = PostListSettingsFileManager.GetSettingsByCampaignId(campaignId) ?? new PublisherPostlistSettingsModel();

            publisherPostlistSettingsModel.CampaignId = publisherPostlistSettingsModel.CampaignId ?? campaignId;

            var objPublisherPostlistSettings = new PublisherPostlistSettings(publisherPostlistSettingsModel);

            var customDialog = new CustomDialog
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objPublisherPostlistSettings
            };

            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetCustomDialog(customDialog, "Postlist Settings");

            objPublisherPostlistSettings.ButtonSave.Click += (senders, events) =>
            {
                try
                {
                    objPublisherPostlistSettings.PublisherPostlistSettingsModel.AddOrUpdateBinFile
                    (objPublisherPostlistSettings.PublisherPostlistSettingsModel);

                    dialogWindow.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            objPublisherPostlistSettings.ButtonCancel.Click += (senders, events) => dialogWindow.Close();

            dialogWindow.ShowDialog();
        }


        public Task<IList<PublisherPostlistModel>> ReadPostList(string campaignId, PostQueuedStatus requiredPostList = PostQueuedStatus.Draft)
        {
            if (string.IsNullOrEmpty(campaignId))
                return null;


            return null;
        }
    }
}