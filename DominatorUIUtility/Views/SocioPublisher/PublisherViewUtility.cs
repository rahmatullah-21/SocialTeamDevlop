using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.Views.SocioPublisher
{
    public class PublisherViewUtility
    {
        public void OpenPostlistSettings()
        {
            var publisherPostlistSettingsModel = new PublisherPostlistSettingsModel();

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
    }
}