using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl : UserControl
    {

        public HeaderControl()
        {
            InitializeComponent();
            mainGrid.DataContext =this;
            IsExpanded = true;
        }
       
 

        public Visibility IsCancelEditVisible
        {
            get
            {
                return (Visibility)GetValue(IsCancelEditVisibleProperty);
            }
            set
            {
                SetValue(IsCancelEditVisibleProperty, value);
            }
        }
        public static readonly DependencyProperty IsCancelEditVisibleProperty =
         DependencyProperty.Register("IsCancelEditVisible", typeof(Visibility), typeof(HeaderControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
         {
             BindsTwoWayByDefault = true
         });


        public string CampaignName
        {
            get
            {
                return (string)GetValue(CampaignNameProperty);
            }
            set
            {
                SetValue(CampaignNameProperty, value);
            }
        }
        public static readonly DependencyProperty CampaignNameProperty =
        DependencyProperty.Register("CampaignName", typeof(string), typeof(HeaderControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
        {
            BindsTwoWayByDefault = true
        });



        public bool IsCampaignNameEditable
        {
            get
            {
                return (bool)GetValue(IsCampaignNameEditableProperty);
            }
            set
            {
                SetValue(IsCampaignNameEditableProperty, value);
            }
        }
        public static readonly DependencyProperty IsCampaignNameEditableProperty =
        DependencyProperty.Register("IsCampaignNameEditable", typeof(bool), typeof(HeaderControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
        {
            BindsTwoWayByDefault = true
        });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Breakpoint here to see if the new value is being set
            var newValue = e.NewValue;
        }




        static readonly RoutedEvent InfoRoutedEvent = EventManager.RegisterRoutedEvent("InfoChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderControl));

        public event RoutedEventHandler InfoChanged
        {
            add { AddHandler(InfoRoutedEvent, value); }
            remove { RemoveHandler(InfoRoutedEvent, value); }
        }

        void RaiseInfoEventHandler()
        {
            RoutedEventArgs objRoutedEventArgs = new RoutedEventArgs(HeaderControl.InfoRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        static readonly RoutedEvent CancelEditRoutedEvent = EventManager.RegisterRoutedEvent("CancelEditClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderControl));

        public event RoutedEventHandler CancelEditClick
        {
            add { AddHandler(CancelEditRoutedEvent, value); }
            remove { RemoveHandler(CancelEditRoutedEvent, value); }
        }

        void CancelEditClickHandler()
        {
            RoutedEventArgs objRoutedEventArgs = new RoutedEventArgs(HeaderControl.CancelEditRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        private void BtnCancelEdit_OnClick(object sender, RoutedEventArgs e)
        {
            CancelEditClickHandler();
        }
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(HeaderControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        private void Info_OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseInfoEventHandler();
        }
    }
    
}
