using System;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherScrapePostViewModel : BindableBase
    {
        private ScrapePostModel _scrapePostModel = new ScrapePostModel();

        public PublisherScrapePostViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            ScrapePostModel = tabItemsControl.ScrapePostModel;
            UploadPostDetailsCommand = new DelegateCommand(UploadPostDetails);
            UploadPostDescriptionCommand = new DelegateCommand(UploadPostDescriptionExecute);
        }


        public ICommand UploadPostDetailsCommand { get; }
        public ICommand UploadPostDescriptionCommand { get; }

        public ScrapePostModel ScrapePostModel
        {
            get => _scrapePostModel;
            set
            {
                if (value == _scrapePostModel)
                    return;
                SetProperty(ref _scrapePostModel, value);
            }
        }

        private void UploadPostDescriptionExecute()
        {
            try
            {
                ScrapePostModel.LstUploadPostDescription = FileUtilities.FileBrowseAndReader();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UploadPostDetails()
        {
            var postDetails = FileUtilities.FileBrowseAndReader();
            if (postDetails.Count > 0)
            {
                postDetails.ForEach(x => ScrapePostModel.LstScrapedPostDetails.Add(x.Trim()));
                ToasterNotification.ShowSuccess("LangKeyPostDetailsUploaded".FromResourceDictionary());
            }
        }
    }
}