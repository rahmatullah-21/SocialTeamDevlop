using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherMediaViewer.xaml
    /// </summary>
    public partial class PublisherMediaViewer : UserControl
    {
        public PublisherMediaViewerViewModel PublisherMediaViewerViewModel
        {
            get { return (PublisherMediaViewerViewModel)GetValue(PublisherMediaViewerViewModelProperty); }
            set { SetValue(PublisherMediaViewerViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PublisherMediaViewerViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PublisherMediaViewerViewModelProperty =
            DependencyProperty.Register("PublisherMediaViewerViewModel",
                typeof(PublisherMediaViewerViewModel),
                typeof(PublisherMediaViewer),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public PublisherMediaViewer()
        {
            InitializeComponent();        
            MediaViewer.DataContext = this;
        }


        public ICommand GoPreviousCommand { get; set; }

        public ICommand GoNextCommand { get; set; }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Breakpoint here to see if the new value is being set
            var newValue = e.NewValue;
        }
    }
}
