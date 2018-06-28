using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherHomeViewModel : BindableBase
    {
        private PublisherHomeModel _publisherHomeModel = new PublisherHomeModel();

        public PublisherHomeModel PublisherHomeModel
        {
            get
            {
                return _publisherHomeModel;
            }
            set
            {               
                if (_publisherHomeModel != null && Equals(_publisherHomeModel, value))
                    return;
                SetProperty(ref _publisherHomeModel, value);
            }
        }

        public void SetDefaultHomePage()
        {
            PublisherHomeModel.SelectedUserControl =  PublisherDefaultPage.Instance();
        } 
    }
}