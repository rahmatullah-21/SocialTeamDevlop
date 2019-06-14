using DominatorHouseCore;
using DominatorHouseCore.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    /// Interaction logic for ActivitySetting.xaml
    /// </summary>
    public partial class ActivitySettingWithoutButton : UserControl
    {
        public ActivitySettingWithoutButton()
        {
            InitializeComponent();
            Setting.DataContext = this;
            DeleteQueryCommand = new DelegateCommand<object>(DeleteQueryExecute);
            DeleteMulipleCommand = new DelegateCommand<object>(DeleteMulipleExecute);
        }
      
        public Speed Model => new Speed();
        
        public bool IsUseGlobalQuery
        {
            get { return (bool)GetValue(IsUseGlobalQueryProperty); }
            set { SetValue(IsUseGlobalQueryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUseGlobalQuery.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUseGlobalQueryProperty =
            DependencyProperty.Register("IsUseGlobalQuery", typeof(bool), typeof(ActivitySettingWithoutButton), new PropertyMetadata(false));

        public JobConfiguration JobConfiguration
        {
            get { return (JobConfiguration)GetValue(JobConfigurationProperty); }
            set { SetValue(JobConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for JobConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationProperty =
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(ActivitySettingWithoutButton), new FrameworkPropertyMetadata()
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
            DependencyProperty.Register("ListQueryType", typeof(List<string>), typeof(ActivitySettingWithoutButton), new PropertyMetadata(new List<string>()));

        public ObservableCollection<QueryInfo> SavedQueries
        {
            get { return (ObservableCollection<QueryInfo>)GetValue(SavedQueriesProperty); }
            set { SetValue(SavedQueriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SavedQueries.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SavedQueriesProperty =
            DependencyProperty.Register("SavedQueries", typeof(ObservableCollection<QueryInfo>), typeof(ActivitySettingWithoutButton), new PropertyMetadata(new ObservableCollection<QueryInfo>()));


        private static readonly DependencyProperty AddQueryCommandProperty
          = DependencyProperty.Register("AddQueryCommand", typeof(ICommand), typeof(ActivitySettingWithoutButton));

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
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ActivitySettingWithoutButton), new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        public ICommand DeleteQueryCommand
        {
            get { return (ICommand)GetValue(DeleteQueryCommandProperty); }
            set { SetValue(DeleteQueryCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteQueryCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteQueryCommandProperty =
            DependencyProperty.Register("DeleteQueryCommand", typeof(ICommand), typeof(ActivitySettingWithoutButton));


        public ICommand DeleteMulipleCommand
        {
            get { return (ICommand)GetValue(DeleteMulipleCommandProperty); }
            set { SetValue(DeleteMulipleCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteMulipleCommandProperty =
            DependencyProperty.Register("DeleteMulipleCommand", typeof(ICommand), typeof(ActivitySettingWithoutButton));

        private static readonly RoutedEvent DeleteQueryEvent = EventManager.RegisterRoutedEvent("DeleteQuery",
                  RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ActivitySettingWithoutButton));

        public event RoutedEventHandler DeleteQuery
        {
            add { AddHandler(DeleteQueryEvent, value); }
            remove { RemoveHandler(DeleteQueryEvent, value); }
        }

        void DeleteQueryEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(DeleteQueryEvent);
            RaiseEvent(routedEventArgs);
        }
        private void DeleteQueryExecute(object sender)
        {
            try
            {
                var QueryToDelete = sender as QueryInfo;
                DeleteQueryEventHandler();
                if (SavedQueries.Any(x => QueryToDelete != null && x.Id == QueryToDelete.Id))
                {
                    SavedQueries.Remove(QueryToDelete);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void DeleteMulipleExecute(object obj)
        {
            try
            {
                foreach (var queryInfo in SavedQueries.ToList())
                {
                    if (queryInfo.IsQuerySelected)
                    {
                        SavedQueries.Remove(queryInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
