using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherDefaultPage.xaml
    /// </summary>   
    public partial class PublisherDefaultPage : UserControl , INotifyPropertyChanged
    {
        private PublisherDefaultPage()
        {
            InitializeComponent();
            PublisherDefault.DataContext = PublisherDefaultViewModel;
        }

        private PublisherDefaultViewModel _publisherDefaultViewModel = new PublisherDefaultViewModel();

        public PublisherDefaultViewModel PublisherDefaultViewModel
        {
            get
            {
                return _publisherDefaultViewModel;
            }
            set
            {
                _publisherDefaultViewModel = value;
                OnPropertyChanged(nameof(PublisherDefaultViewModel));
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
