using System.Diagnostics;
using DominatorHouseCore.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using LegionUIUtility.Behaviours;
using Prism.Commands;

namespace LegionUIUtility.CustomControl
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
            SaveCommand = new BaseCommand<object>(CanExecute, Execute);
            TutorialCommand = new DelegateCommand<string>(TutorialExecute);
            IsExpanded = true;
            AccountItemSource = new ObservableCollectionBase<string>();
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
            // ReSharper disable once UnusedVariable
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

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(AccountGrowthModeHeader), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        private void ClpsExpnd_OnClick(object sender, RoutedEventArgs e)
        {
            HeaderHelper.ExpandCollapseAllExpander(sender, IsExpanded);
        }
        private static readonly DependencyProperty SaveCommandProperty
            = DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(AccountGrowthModeHeader));

        public ICommand SaveCommand
        {
            get
            {
                return (ICommand)GetValue(SaveCommandProperty);
            }
            set
            {
                SetValue(SaveCommandProperty, value);
            }
        }

        public bool CanExecute(object sender)
        {
            return true;
        }

        public void Execute(object sender)
        {
            SaveEventArgsHandler();
        }


        public ICommand SelectionChangedCommand
        {
            get { return (ICommand)GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionChangedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionChangedCommandProperty =
            DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(AccountGrowthModeHeader));

        public string VideoTutorialLink
        {
            get { return (string)GetValue(VideoTutorialLinkProperty); }
            set { SetValue(VideoTutorialLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoTutorialLinkProperty =
            DependencyProperty.Register("VideoTutorialLink", typeof(string), typeof(AccountGrowthModeHeader), new PropertyMetadata(string.Empty));

        public ICommand TutorialCommand
        {
            get { return (ICommand)GetValue(TutorialCommandProperty); }
            set { SetValue(TutorialCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TutorialCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TutorialCommandProperty =
            DependencyProperty.Register("TutorialCommand", typeof(ICommand), typeof(AccountGrowthModeHeader));

        private void TutorialExecute(string tutorialLink)
        {
            Process.Start(tutorialLink);
        }
    }

}
