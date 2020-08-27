using DominatorUIUtility.Behaviours;
using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl
    {

        public HeaderControl()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
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
         DependencyProperty.Register("IsCancelEditVisible", typeof(Visibility), typeof(HeaderControl), new FrameworkPropertyMetadata()
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
        DependencyProperty.Register("CampaignName", typeof(string), typeof(HeaderControl), new FrameworkPropertyMetadata()
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
        DependencyProperty.Register("IsCampaignNameEditable", typeof(bool), typeof(HeaderControl), new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true
        });


        static readonly RoutedEvent InfoRoutedEvent = EventManager.RegisterRoutedEvent("InfoChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderControl));

        public event RoutedEventHandler InfoChanged
        {
            add { AddHandler(InfoRoutedEvent, value); }
            remove { RemoveHandler(InfoRoutedEvent, value); }
        }

        static readonly RoutedEvent CancelEditRoutedEvent = EventManager.RegisterRoutedEvent("CancelEditClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderControl));

        public event RoutedEventHandler CancelEditClick
        {
            add { AddHandler(CancelEditRoutedEvent, value); }
            remove { RemoveHandler(CancelEditRoutedEvent, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(HeaderControl), new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true
            });
        
        private void ClpsExpnd_OnClick(object sender, RoutedEventArgs e)
        {
            HeaderHelper.ExpandCollapseAllExpander(sender, IsExpanded);
        }


        public ICommand CancelEditCommand
        {
            get { return (ICommand)GetValue(CancelEditCommandProperty); }
            set { SetValue(CancelEditCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelEditCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelEditCommandProperty =
            DependencyProperty.Register("CancelEditCommand", typeof(ICommand), typeof(HeaderControl));



        public ICommand InfoCommand
        {
            get { return (ICommand)GetValue(InfoCommandProperty); }
            set { SetValue(InfoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InfoCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InfoCommandProperty =
            DependencyProperty.Register("InfoCommand", typeof(ICommand), typeof(HeaderControl));


    }

}
