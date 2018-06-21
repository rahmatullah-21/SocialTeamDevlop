using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for SelectDestinations.xaml
    /// </summary>
    public partial class SelectDestinations : UserControl, INotifyPropertyChanged
    {
        public SelectDestinations()
        {
            InitializeComponent();
            ManageDestination.DataContext = PublisherManageDestinationViewModel;
        }

        public SelectDestinations(ObservableCollection<string> LstDestinationId):this()
        {
            _publisherManageDestinationViewModel.ListPublisherManageDestinationModels?.ToList().ForEach(x =>
            {
                x.IsSelected = LstDestinationId.Contains(x.DestinationId);
            });
        }
        public PublisherManageDestinationViewModel PublisherManageDestinationViewModel
        {
            get
            {
                return _publisherManageDestinationViewModel;
            }
            set
            {
                _publisherManageDestinationViewModel = value;
                OnPropertyChanged(nameof(PublisherManageDestinationViewModel));
            }
        }


        private PublisherManageDestinationViewModel _publisherManageDestinationViewModel = new PublisherManageDestinationViewModel();
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

      
    }
}
