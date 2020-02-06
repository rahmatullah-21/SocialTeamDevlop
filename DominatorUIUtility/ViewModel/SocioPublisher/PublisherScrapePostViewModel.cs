using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Prism.Commands;
using System.Windows.Input;
using System;
using DominatorHouseCore;

namespace LegionUIUtility.ViewModel.SocioPublisher
{
    public class PublisherScrapePostViewModel : BindableBase
    {
        public PublisherScrapePostViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            ScrapePostModel = tabItemsControl.ScrapePostModel;
            UploadPostDetailsCommand = new DelegateCommand(UploadPostDetails);
            UploadPostDescriptionCommand = new DelegateCommand(UploadPostDescriptionExecute);
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

        
        public ICommand UploadPostDetailsCommand { get; }
        public ICommand UploadPostDescriptionCommand { get; }
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
            if (postDetails.Count > 0)
            {
                postDetails.ForEach(x => ScrapePostModel.LstScrapedPostDetails.Add(x.Trim()));
                ToasterNotification.ShowSuccess("LangKeyPostDetailsUploaded".FromResourceDictionary());
            }
        }
    }
}