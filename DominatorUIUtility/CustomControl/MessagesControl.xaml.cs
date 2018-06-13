using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for MessagesControl.xaml
    /// </summary>
    public partial class MessagesControl : UserControl
    {
        public MessagesControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }
        private static readonly RoutedEvent AddMessagesToListEvent =
     EventManager.RegisterRoutedEvent("AddMessagesToListChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
         typeof(MessagesControl));

        public event RoutedEventHandler AddMessagesToListChanged
        {
            add { AddHandler(AddMessagesToListEvent, value); }
            remove { RemoveHandler(AddMessagesToListEvent, value); }
        }

        void AddCommentToListEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddMessagesToListEvent);
            RaiseEvent(routedEventArgs);
        }


        public ManageMessagesModel Messages
        {
            get { return (ManageMessagesModel)GetValue(ManageMessagesProperty); }
            set { SetValue(ManageMessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageMessagesProperty =
            DependencyProperty.Register("Messages", typeof(ManageMessagesModel), typeof(MessagesControl), new PropertyMetadata(OnAvailableItemsChanged));
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get { return (ObservableCollection<ManageMessagesModel>)GetValue(LstManageMessagesModelProperty); }
            set { SetValue(LstManageMessagesModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>), typeof(MessagesControl), new PropertyMetadata(new ObservableCollection<ManageMessagesModel>()));

        private void BtnAddMessagesToList_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Messages.MessagesText))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please type some message !!");
                return;
            }
            
            AddCheckedQueryToList();
            if (Messages.SelectedQuery.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please add atleast one query!!");
                return;
            }
            if (btnAddMessagesToList.Content.ToString() == "Update Message")
            {
                LstManageMessagesModel.Select(x =>
                {
                    if (x.SerialNo == Messages.SerialNo)
                    {
                        x.MessagesText = Messages.MessagesText;
                        x.LstQueries = Messages.LstQueries;
                    }
                    return x;
                });
                Messages.SelectedQuery.Remove(Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Messages.LstQueries.Select(x => { x.IsContentSelected = false; return x; }).ToList();
                Window.GetWindow(this).Close();
            }
            else
            {
                AddCommentToListEventHandler();
            }

        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            if (((QueryContent)(sender as CheckBox).DataContext).Content.QueryValue == "All")
            {
                Messages.LstQueries.ToList().Select(query => { query.IsContentSelected = IsChecked; return query; }).ToList();
            }

        }
        private void AddCheckedQueryToList()
        {
            Messages.SelectedQuery.Clear();
            Messages.LstQueries.ToList().ForEach(query =>
            {
                if (query.IsContentSelected)
                    Messages.SelectedQuery.Add(query);
            });
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }
    }
}
