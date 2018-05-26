using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherMediaViewer.xaml
    /// </summary>
    public partial class PublisherMediaViewer : UserControl , INotifyPropertyChanged
    {
        //public PublisherMediaViewerViewModel PublisherMediaViewerViewModel
        //{
        //    get
        //    {
        //        return (PublisherMediaViewerViewModel)GetValue(PublisherMediaViewerViewModelProperty);
        //    }
        //    set
        //    {
        //        SetValue(PublisherMediaViewerViewModelProperty, value);
        //    }
        //}

        //// Using a DependencyProperty as the backing store for PublisherMediaViewerViewModel.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PublisherMediaViewerViewModelProperty =
        //    DependencyProperty.Register("PublisherMediaViewerViewModel",
        //        typeof(PublisherMediaViewerViewModel),
        //        typeof(PublisherMediaViewer),
        //        new FrameworkPropertyMetadata(OnAvailableItemsChanged)
        //        {
        //            BindsTwoWayByDefault = true
        //        });

        private PublisherMediaViewerViewModel _publisherMediaViewerViewModel= new PublisherMediaViewerViewModel();

        public PublisherMediaViewerViewModel PublisherMediaViewerViewModel
        {
            get
            {
                return _publisherMediaViewerViewModel;
            }
            set
            {
                if(_publisherMediaViewerViewModel == value)
                    return;
                _publisherMediaViewerViewModel = value;
                OnPropertyChanged(nameof(PublisherMediaViewerViewModel));
            }
        }

        public PublisherMediaViewer()
        {
            InitializeComponent();        
            MediaViewer.DataContext = PublisherMediaViewerViewModel;
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Breakpoint here to see if the new value is being set
            var newValue = e.NewValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
