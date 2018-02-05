using DominatorHouseCore.Utility;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountGrowthModeHeader.xaml
    /// </summary>
    public partial class AccountGrowthModeHeader : UserControl
    {
        public AccountGrowthModeHeader()
        {
            InitializeComponent();

            mainGrid.DataContext = this;

        }

        public ObservableCollectionBase<string> AccountItemSource
        {
            get { return (ObservableCollectionBase<string>)GetValue(AccountItemSourceProperty); }
            set { SetValue(AccountItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AccountItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccountItemSourceProperty =
            DependencyProperty.Register("AccountItemSource", typeof(ObservableCollectionBase<string>), typeof(AccountGrowthModeHeader), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public string SelectedItem
        {
            get
            {
                return (string)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }
        public static readonly DependencyProperty SelectedItemProperty =
       DependencyProperty.Register("SelectedItem", typeof(string), typeof(AccountGrowthModeHeader), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
       {
           BindsTwoWayByDefault = true
       });

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }
        public static readonly DependencyProperty SelectedIndexProperty =
       DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AccountGrowthModeHeader), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
       {
           BindsTwoWayByDefault = true
       });
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        static readonly RoutedEvent SelectionChangedRoutedEvent = EventManager.RegisterRoutedEvent("SelectionChangedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AccountGrowthModeHeader));
        
        public event RoutedEventHandler SelectionChangedEvent
        {
            add { AddHandler(SelectionChangedRoutedEvent, value); }
            remove { RemoveHandler(SelectionChangedRoutedEvent, value); }
        }
        void SelectionChangedEventHandler()
        {
            RoutedEventArgs objRoutedEventArgs = new RoutedEventArgs(SelectionChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler();
        }
        private static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent("SaveClick", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(AccountGrowthModeHeader));

        public event RoutedEventHandler SaveClick
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }

        void SaveEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveEvent);
            RaiseEvent(rountedargs);

        }
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveEventArgsHandler();
        }
    }
}
