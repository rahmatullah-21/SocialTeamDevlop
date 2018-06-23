using System;
using System.IO;
using System.Linq;
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
        public ICommand ExportSelectedCommand { get; set; }

        #endregion

        #region Export

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {
            var selectedPost = PublisherPostlist.Where(x => x.IsPostlistSelected).ToList();

            if (selectedPost.Count != 0)
            {
                var exportPath = FileUtilities.GetExportPath();

                if (!string.IsNullOrEmpty(exportPath))
                {
                    var header =
                        "Post Description,MediaList,ShareUrl,ExpiredTime,Published,Running Status";

                    var filename = $"{exportPath}\\{PublisherPostlist.Select(x=>x.CampaignId).FirstOrDefault()}.csv";

                    FileUtilities.AddHeaderToCsv(filename, header);

                    selectedPost.ForEach(post =>
                    {
                        var mediaUrls = string.Empty;
                        post.MediaList.ForEach(x => { mediaUrls += x +ConstantVariable.Separator; });
                       var csvData = post.PostDescription + "," + mediaUrls + "," + post.ShareUrl + "," +
                                      post.ExpiredTime + ","
                                      + post.ExpiredTime + "," + post.PostRunningStatus ;
                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    });
                }
                else
                {
                    Dialog.ShowDialog("Warning", "Please select path to export.");
                }

            }
            else
            {
                Dialog.ShowDialog("Warning", "Please select atleast one post to export.");
            }
        }

        #endregion


        #region  PublishedDetails

        private bool PublishedDetailsCanExecute(object sender) => true;

        private void PublishedDetailsExecute(object sender)
        {
            try
            {
                var currentData = sender as PublisherPostlistModel;

                if (currentData?.LstPublishedPostDetailsModels.Count != 0)
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
                if (currentData?.LstPublishedPostDetailsModels.Count != 0)
                {
                    var exportPath = FileUtilities.GetExportPath();

                    if (!string.IsNullOrEmpty(exportPath))
                    {
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
                            using (var streamWriter = new StreamWriter(filename, true))
                            {
                                streamWriter.WriteLine(csvData);
                            }
                        });
                    }
                    else
                    {
                        Dialog.ShowDialog("Warning", "Please select path to export.");
                    }
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
