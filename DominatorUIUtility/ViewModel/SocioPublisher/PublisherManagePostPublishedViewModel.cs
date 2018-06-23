using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPublishedViewModel : PublisherPostlistBaseViewModel
    {
        public PublisherManagePostPublishedViewModel()
        {
            ExportCommand = new BaseCommand<object>(ExportCanExecute, ExportExecute);
            PublishedDetailsCommand = new BaseCommand<object>(PublishedDetailsCanExecute, PublishedDetailsExecute);
            ReportCommand = new BaseCommand<object>(ReportCanExecute, ReportExecute);
        }


        #region Properties

        public ICommand ExportCommand { get; set; }
        public ICommand PublishedDetailsCommand { get; set; }
        public ICommand ReportCommand { get; set; }

        #endregion

        #region Export

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {

        }

        #endregion


        #region  PublishedDetails

        private bool PublishedDetailsCanExecute(object sender) => true;

        private void PublishedDetailsExecute(object sender)
        {
            try
            {
                var currentData = sender as PublisherPostlistModel;

                if (currentData?.LstPublishedPostDetailsModels.Count!=0)
                {
                    Dialog dialog = new Dialog();
                    PublishedPostDetails publishedPostDetails = new PublishedPostDetails(currentData);
                    var window = dialog.GetMetroWindow(publishedPostDetails, "Published Details");
                    window.ShowDialog(); 
                }
                else
                {
                    Dialog.ShowDialog("Published Details", "No Details available.");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion


        #region Report

        private bool ReportCanExecute(object sender) => true;

        private void ReportExecute(object sender)
        {
            try
            {
                var currentData = sender as PublisherPostlistModel;
                if (currentData?.LstPublishedPostDetailsModels.Count!=0)
                {
                    var exportPath = FileUtilities.GetExportPath();
                    var header =
                        "Account Name,Destination,Destination Url,Description,Published,Successful,Published Date,Link";

                    var filename = $"{exportPath}\\{currentData?.PostId}.csv";

                    FileUtilities.AddHeaderToCsv(filename, header);

                    currentData?.LstPublishedPostDetailsModels.ForEach(post =>
                    {
                        var csvData = post.AccountName + "," + post.Destination + "," + post.DestinationUrl + "," +
                                      post.Description + ","
                                      + post.IsPublished + "," + post.Successful + "," + post.PublishedDate + "," +
                                      post.Link;
                        using (var streamWriter = new StreamWriter(filename, false))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }); 
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

      
        #endregion
    }
}
