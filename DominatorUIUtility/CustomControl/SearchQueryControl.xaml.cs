using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.LogHelper;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.IO;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Diagnostics;

namespace DominatorUIUtility.CustomControl
{
    public partial class SearchQueryControl : UserControl, INotifyPropertyChanged
    {
        public SearchQueryControl()
        {
            InitializeComponent();
            CurrentQuery = new QueryInfo();
            MainGrid.DataContext = this;
            IsExpanded = true;
            // AddQueryCommand = new BaseCommand<object>(CanExecute, Execute);
            SelectedIndex = 0;
            ListQueryType = new List<string>();
            ListQueryInfo = new ObservableCollection<QueryInfo>();
            LstNonQueryType.Add("LangKeyOwnFollowers".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyOwnFollowings".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyNewsfeed".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyJoinedCommunityMembers".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyMyConnectionsPostS".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyScrapUsersWhoMessagedUs".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyScrapAllLikes".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyNewsFeedPosts".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyOwnFriends".FromResourceDictionary());
            LstNonQueryType.Add("LangKeyScrapUserWhomWeMessaged".FromResourceDictionary());

            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQueryExecute);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMulipleExecute);

        }


        #region Variables

        private int _selectedIndex;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        private bool _isAllQuerySelected;

        public bool IsAllQuerySelected
        {
            get
            {
                return _isAllQuerySelected;
            }
            set
            {
                if (_isAllQuerySelected == value)
                    return;
                _isAllQuerySelected = value;
                OnPropertyChanged(nameof(IsAllQuerySelected));
                SelectAll(_isAllQuerySelected);
                IsCheckFromList = false;
            }
        }

        public bool IsCheckFromList { get; set; }
        private void SelectAll(bool _isAllQuerySelected)
        {
            if (IsCheckFromList)
                return;
            ListQueryInfo.Select(x =>
            {
                x.IsQuerySelected = _isAllQuerySelected;
                return x;
            }).ToList();
        }


        public List<Enum> LstQueryType
        {
            get { return (List<Enum>)GetValue(LstQueryTypeProperty); }
            set { SetValue(LstQueryTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstQueryTypeProperty =
            DependencyProperty.Register("LstQueryType", typeof(List<Enum>),
                typeof(SearchQueryControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)

                {
                    BindsTwoWayByDefault = true
                });


        public IEnumerable<string> ListQueryType
        {
            get { return (IEnumerable<string>)GetValue(ListQueryTypeProperty); }
            set { SetValue(ListQueryTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListQueryTypeProperty =
            DependencyProperty.Register("ListQueryType", typeof(IEnumerable<string>), typeof(SearchQueryControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public QueryInfo CurrentQuery
        {
            get { return (QueryInfo)GetValue(CurrentQueryProperty); }
            set { SetValue(CurrentQueryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentQuery.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentQueryProperty =
            DependencyProperty.Register("CurrentQuery", typeof(QueryInfo), typeof(SearchQueryControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });



        public ObservableCollection<QueryInfo> ListQueryInfo
        {
            get { return (ObservableCollection<QueryInfo>)GetValue(ListQueryInfoProperty); }
            set { SetValue(ListQueryInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListQueryInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListQueryInfoProperty =
            DependencyProperty.Register("ListQueryInfo", typeof(ObservableCollection<QueryInfo>), typeof(SearchQueryControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public List<string> QueryCollection { get; set; } = new List<string>();

        #endregion

        #region Import Query Details

        /// <summary>
        /// Create a routed event which is registered to event manager with the characteristics 
        /// </summary>
        private static readonly RoutedEvent GetQueryClickEvent = EventManager.RegisterRoutedEvent("GetQueryClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchQueryControl));

        /// <summary>
        /// Create a RoutedEventHandler for query clicks
        /// </summary>
        public event RoutedEventHandler GetQueryClick
        {
            add { AddHandler(GetQueryClickEvent, value); }
            remove { RemoveHandler(GetQueryClickEvent, value); }
        }

        void GetQueryClickEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(GetQueryClickEvent);
            RaiseEvent(routedEventArgs);
        }


        /// <summary>
        /// To Read the Query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImportQuery_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                QueryCollection.Clear();
                QueryCollection.AddRange(FileUtilities.FileBrowseAndReader());

                if (QueryCollection.Count != 0)
                {
                    Dialog.ShowDialog("Info", "Queries are ready to add !!");
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, "", ActivityType, "Query sucessfully uploaded !!");
                }
                else
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, "", ActivityType, "You did not upload any query !!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, "", ActivityType, "There is error in uploading query !!");
            }

            GetQueryClickEventHandler();
        }

        #endregion

        #region CustomFilters

        private static readonly RoutedEvent CustomFilterChangedEvent =
            EventManager.RegisterRoutedEvent("CustomFilterChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(SearchQueryControl));

        public event RoutedEventHandler CustomFilterChanged
        {
            add { AddHandler(CustomFilterChangedEvent, value); }
            remove { RemoveHandler(CustomFilterChangedEvent, value); }
        }

        void CustomFilterEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(CustomFilterChangedEvent);
            RaiseEvent(routedEventArgs);
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            CustomFilterEventHandler();
        }

        #endregion

        #region Add current query to query list 

        private static readonly RoutedEvent AddQueryEvent = EventManager.RegisterRoutedEvent("AddQuery",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchQueryControl));

        public event RoutedEventHandler AddQuery
        {
            add { AddHandler(AddQueryEvent, value); }
            remove { RemoveHandler(AddQueryEvent, value); }
        }
        void AddQueryEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddQueryEvent);
            RaiseEvent(routedEventArgs);
        }

        private static readonly DependencyProperty AddQueryCommandProperty
            = DependencyProperty.Register("AddQueryCommand", typeof(ICommand), typeof(SearchQueryControl));

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

        #endregion

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SearchQueryControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged));


        #region Delete the query from query list

        private static readonly RoutedEvent DeleteQueryEvent = EventManager.RegisterRoutedEvent("DeleteQuery",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchQueryControl));

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

        private void DeleteSingle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentQuery = ((FrameworkElement)sender).DataContext as QueryInfo;
            DeleteQueryEventHandler();
            if (ListQueryInfo.Any(x => CurrentQuery != null && x.Id == CurrentQuery.Id))
            {
                QueryCollection.Remove(CurrentQuery.QueryValue);
                ListQueryInfo.Remove(CurrentQuery);
            }
        }

        #endregion

        #region Selected Query Type Changed
        private void CmbboxQueryTypeLists_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                CurrentQuery.QueryType = ListQueryType.ToList()[SelectedIndex];
                if (LstNonQueryType.Contains(CurrentQuery.QueryType))
                {
                    TxtInputQuery.Text = "NA";
                    TxtInputQuery.IsEnabled = false;
                }
                else
                {
                    TxtInputQuery.IsEnabled = true;
                    TxtInputQuery.Clear();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SearchQueryControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnQuerySelect(object sender, RoutedEventArgs e)
        {
            if (ListQueryInfo.All(x => x.IsQuerySelected))
                IsAllQuerySelected = true;
            else
            {
                if (IsAllQuerySelected)
                    IsCheckFromList = true;
                IsAllQuerySelected = false;

            }
        }

        public HashSet<string> LstNonQueryType { get; set; } = new HashSet<string>();

        private bool _isEnable = true;

        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }
            set
            {
                _isEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }
        public ActivityType ActivityType { get; set; }

        public ICommand CustomFilterCommand
        {
            get { return (ICommand)GetValue(CustomFilterCommandProperty); }
            set { SetValue(CustomFilterCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomFilterCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomFilterCommandProperty =
            DependencyProperty.Register("CustomFilterCommand", typeof(ICommand), typeof(SearchQueryControl));

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CurrentQuery.QueryType = ListQueryType.ToList()[SelectedIndex];
                if (LstNonQueryType.Contains(CurrentQuery.QueryType))
                {
                    CurrentQuery.QueryValue = "NA";
                    IsEnable = false;
                    BtnImportQuery.IsEnabled = false;
                }
                else
                {
                    IsEnable = true;
                    BtnImportQuery.IsEnabled = true;
                    CurrentQuery.QueryValue = string.Empty;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ICommand DeleteQueryCommand
        {
            get { return (ICommand)GetValue(DeleteQueryCommandProperty); }
            set { SetValue(DeleteQueryCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteQueryCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteQueryCommandProperty =
            DependencyProperty.Register("DeleteQueryCommand", typeof(ICommand), typeof(SearchQueryControl));

        private void DeleteQueryExecute(object sender)
        {
            try
            {
                var QueryToDelete = sender as QueryInfo;
                DeleteQueryEventHandler();
                if (ListQueryInfo.Any(x => QueryToDelete != null && x.Id == QueryToDelete.Id))
                {
                    QueryCollection.Remove(QueryToDelete.QueryValue);
                    ListQueryInfo.Remove(QueryToDelete);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public ICommand DeleteMulipleCommand
        {
            get { return (ICommand)GetValue(DeleteMulipleCommandProperty); }
            set { SetValue(DeleteMulipleCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteMulipleCommandProperty =
            DependencyProperty.Register("DeleteMulipleCommand", typeof(ICommand), typeof(SearchQueryControl));


        private void DeleteMulipleExecute(object obj)
        {
            try
            {
                if (IsAllQuerySelected)
                {
                    QueryCollection.Clear();
                    ListQueryInfo.Clear();
                    IsAllQuerySelected = false;
                    return;
                }
                foreach (var queryInfo in ListQueryInfo.ToList())
                {
                    if (queryInfo.IsQuerySelected)
                    {
                        QueryCollection.Remove(queryInfo.QueryValue);
                        ListQueryInfo.Remove(queryInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ExportSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ListQueryInfo.Any(x => x.IsQuerySelected))
                {
                    Dialog.ShowDialog("Error", "Please select atleast one query.");
                    return;
                }
                var network = SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social ? SocinatorInitialize.AccountModeActiveSocialNetwork : SocinatorInitialize.ActiveSocialNetwork;
                SaveFileDialog saveFiledialog = new SaveFileDialog
                {
                    Filter = "CSV file (.csv)|*.csv",
                    FileName = network + "-" + ActivityType.ToString() + "-Query-" + DateTimeUtilities.GetCurrentEpochTime(DateTime.Now)
                };

                if (saveFiledialog.ShowDialog() == true)
                {
                    string filename = saveFiledialog.FileName;
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        ListQueryInfo.ForEach(x =>
                        {
                            if (x.IsQuerySelected)
                            {
                                streamWriter.WriteLine(network + "," + ActivityType.ToString() + "," + x.QueryType + "," + x.QueryValue);
                            }
                        });
                        ToasterNotification.ShowSuccess("Query successfully exported.");
                    }

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public ICommand QuryTypeSelectionChangedCommand
        {
            get { return (ICommand)GetValue(QuryTypeSelectionChangedCommandProperty); }
            set { SetValue(QuryTypeSelectionChangedCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuryTypeSelectionChangedCommandProperty =
            DependencyProperty.Register("QuryTypeSelectionChangedCommand", typeof(ICommand), typeof(SearchQueryControl));


        public object SelectionChangedCommandParameter
        {
            get { return (object)GetValue(SelectionChangedCommandParameterProperty); }
            set { SetValue(SelectionChangedCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionChangedCommandParameterProperty =
            DependencyProperty.Register("SelectionChangedCommandParameter", typeof(object), typeof(SearchQueryControl));

        private void SearchQueries_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ListQueryInfo.Count > 0)
                SearchQueries.ScrollIntoView(ListQueryInfo[0]);
        }
    }


}
