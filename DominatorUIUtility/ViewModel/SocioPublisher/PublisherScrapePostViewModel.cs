using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherScrapePostViewModel : BindableBase
    {
        public PublisherScrapePostViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            ScrapePostModel = tabItemsControl.ScrapePostModel;
        }
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
    }
}