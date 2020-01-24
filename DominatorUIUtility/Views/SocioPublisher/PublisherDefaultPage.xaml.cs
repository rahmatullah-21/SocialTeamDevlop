using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using LegionUIUtility.ViewModel.SocioPublisher;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace LegionUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherDefaultPage.xaml
    /// </summary>   
    public partial class PublisherDefaultPage : UserControl, INotifyPropertyChanged
    {
        private PublisherDefaultPage()
        {
            InitializeComponent();
            PublisherDefault.DataContext = PublisherDefaultViewModel;
        }

        private readonly PublisherDefaultViewModel _publisherDefaultViewModel =
            ServiceLocator.Current.GetInstance<PublisherDefaultViewModel>();

        public PublisherDefaultViewModel PublisherDefaultViewModel
        {
            get
            {
                return _publisherDefaultViewModel;
            }
        }

        private static PublisherDefaultPage _indexPage;
      
        public static PublisherDefaultPage Instance()
        {
            if (_indexPage == null)
            {
                _indexPage = new PublisherDefaultPage();
            }
            _indexPage.PublisherDefaultViewModel.InitializeDefaultCampaignStatus();
            return _indexPage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}
