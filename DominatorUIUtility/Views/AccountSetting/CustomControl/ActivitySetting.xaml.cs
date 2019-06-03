using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel.Startup;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    /// Interaction logic for ActivitySetting.xaml
    /// </summary>
    public partial class ActivitySetting : UserControl
    {

        public ActivitySetting()
        {
            InitializeComponent();
            Setting.DataContext = this;

        }
        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Heading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register("Heading", typeof(string), typeof(ActivitySetting), new PropertyMetadata(string.Empty));

        public JobConfiguration JobConfiguration
        {
            get { return (JobConfiguration)GetValue(JobConfigurationProperty); }
            set { SetValue(JobConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for JobConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationProperty =
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(ActivitySetting), new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });



        public List<string> ListQueryType
        {
            get { return (List<string>)GetValue(ListQueryTypeProperty); }
            set { SetValue(ListQueryTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListQueryTypeProperty =
            DependencyProperty.Register("ListQueryType", typeof(List<string>), typeof(ActivitySetting), new PropertyMetadata(new List<string>()));



        public ObservableCollection<QueryInfo> SavedQueries
        {
            get { return (ObservableCollection<QueryInfo>)GetValue(SavedQueriesProperty); }
            set { SetValue(SavedQueriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SavedQueries.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SavedQueriesProperty =
            DependencyProperty.Register("SavedQueries", typeof(ObservableCollection<QueryInfo>), typeof(ActivitySetting), new PropertyMetadata(new ObservableCollection<QueryInfo>()));



        public ICommand NextCommand
        {
            get { return (ICommand)GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(ActivitySetting));
        public ICommand PreviousCommand
        {
            get { return (ICommand)GetValue(PreviousCommandProperty); }
            set { SetValue(PreviousCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(ActivitySetting));

        private static readonly DependencyProperty AddQueryCommandProperty
          = DependencyProperty.Register("AddQueryCommand", typeof(ICommand), typeof(ActivitySetting));

        public ICommand AddQueryCommand
        {
            get
            {
                return (ICommand)GetValue(AddQueryCommandProperty);
            }
            set
            {
                SetValue(AddQueryCommandProperty, value);
            }
        }



        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ActivitySetting), new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public string NextButtonContent
        {
            get { return (string)GetValue(NextButtonContentProperty); }
            set { SetValue(NextButtonContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextButtonContentProperty =
            DependencyProperty.Register("NextButtonContent", typeof(string), typeof(ActivitySetting), new FrameworkPropertyMetadata("LangKeyNext".FromResourceDictionary())
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public Visibility PreviousVisiblity
        {
            get { return (Visibility)GetValue(PreviousVisiblityProperty); }
            set { SetValue(PreviousVisiblityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousVisiblityProperty =
            DependencyProperty.Register("PreviousVisiblity", typeof(Visibility), typeof(ActivitySetting), new PropertyMetadata(Visibility.Visible));

        private void SaveData(object sender, RoutedEventArgs e)
        {
            if (NextButtonContent == "LangKeyFinish".FromResourceDictionary())
            {
                var saveSetting = ServiceLocator.Current.GetInstance<ISaveSetting>();
                saveSetting.Save();
                var mainViewModel = ServiceLocator.Current.GetInstance<IMainViewModel>();
                mainViewModel.IsPopUpOpen = false;
            }
        }

    }
}
