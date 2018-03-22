using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;

namespace DominatorUIUtility.CustomControl
{    
    public partial class SearchQueryControl : UserControl
    {

        public SearchQueryControl()
        {
            InitializeComponent();           
            CurrentQuery = new QueryInfo();          
            MainGrid.DataContext = this;
            IsExpanded = true;
         AddQueryCommand = new BaseCommand<object>(CanExecute, Execute);
           
        }

        #region Variables

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



        public ObservableCollectionBase<QueryInfo> ListQueryInfo
        {
            get { return (ObservableCollectionBase<QueryInfo>)GetValue(ListQueryInfoProperty); }
            set { SetValue(ListQueryInfoProperty, value); }
        }



        // Using a DependencyProperty as the backing store for ListQueryInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListQueryInfoProperty =
            DependencyProperty.Register("ListQueryInfo", typeof(ObservableCollectionBase<QueryInfo>), typeof(SearchQueryControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
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
            QueryCollection.AddRange(FileUtilities.FileBrowseAndReader());
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


        private void btnAddToList_Click(object sender, RoutedEventArgs e)
        {
            CurrentQuery.QueryValue= TxtInputQuery.Text.ToString();
            TxtInputQuery.Text = string.Empty;
            CmbboxQueryTypeLists.SelectedIndex = 0;
            AddQueryEventHandler();
        }

        #endregion

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

        private void DeleteSingle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var currentRow = ((FrameworkElement)sender).DataContext as QueryInfo;

            if (ListQueryInfo.Any(x => currentRow != null && x.Id == currentRow.Id))
            {
                ListQueryInfo.Remove(currentRow);
            }
            DeleteQueryEventHandler();
        }

        #endregion

        #region Selected Query Type Changed
        private void CmbboxQueryTypeLists_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                var selectedvalue = (ComboBox)(FrameworkElement)sender;

                if (selectedvalue != null)
                    CurrentQuery.QueryType = selectedvalue.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            CurrentQuery.QueryValue = TxtInputQuery.Text.ToString();
            CurrentQuery.QueryType = CmbboxQueryTypeLists.SelectedItem.ToString();
            TxtInputQuery.Text = string.Empty;
            CmbboxQueryTypeLists.SelectedIndex = 0;
            AddQueryEventHandler();
        }

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
    }

    
}
