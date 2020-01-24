using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using LegionUIUtility.Views.SocioPublisher;
using System.ComponentModel;

namespace LegionUIUtility.ViewModel.SocioPublisher
{
    public interface IPublisherHomeViewModel : INotifyPropertyChanged
    {

    }

    public class PublisherHomeViewModel : BindableBase, IPublisherHomeViewModel
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