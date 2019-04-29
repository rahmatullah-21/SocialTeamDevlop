using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Prism.Commands;
using System.Windows.Input;
using System;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherScrapePostViewModel : BindableBase
    {
        public PublisherScrapePostViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            ScrapePostModel = tabItemsControl.ScrapePostModel;
            UploadPostDetailsCommand = new DelegateCommand(UploadPostDetails);
        }

        public ICommand UploadPostDetailsCommand { get; }
        private ScrapePostModel _scrapePostModel = new ScrapePostModel();

        public ScrapePostModel ScrapePostModel
        {
            get
            {
                return _scrapePostModel;
            }
            set
            {
                if (value == _scrapePostModel)
                    return;
                SetProperty(ref _scrapePostModel, value);
            }
        }

        private void UploadPostDetails()
        {
            var postDetails = FileUtilities.FileBrowseAndReader();
            if(postDetails.Count>0)
            {
                ScrapePostModel.LstScrapedPostDetails.AddRange(postDetails);
                ToasterNotification.ShowSuccess("Post details successfully uploaded.");
            }
        }
    }
}