using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace LegionUIUtility.ViewModel.SocioPublisher
{
    public class PublisherSharePostViewModel : BindableBase
    {
        public PublisherSharePostViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {

            SharePostModel = tabItemsControl.SharePostModel;

        }
        private SharePostModel _sharePostModel = new SharePostModel();

        public SharePostModel SharePostModel
        {
            get
            {
                return _sharePostModel;
            }
            set
            {
                if (value == _sharePostModel)
                    return;
                SetProperty(ref _sharePostModel, value);
            }
        }
    }
}