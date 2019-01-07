using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;
using DominatorHouseCore.Command;

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
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessagesExecute);

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
                    if (x.MessageId == Messages.MessageId)
                    {
                        x.MessagesText = Messages.MessagesText;
                        x.LstQueries = Messages.LstQueries;
                    }
                    return x;
                }).ToList();
                Messages.SelectedQuery.Remove(Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Messages.LstQueries.Select(x => { x.IsContentSelected = false; return x; }).ToList();
                Isupdated = true;

                Dialog.CloseDialog(this);
            }
            else
            {
                AddCommentToListEventHandler();
            }

        }

        private bool _isUncheckfromList;
        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            var currentQuery = ((QueryContent)(sender as CheckBox).DataContext).Content.QueryValue;
            if (!Messages.LstQueries.Skip(1).All(x => x.IsContentSelected))
            {

                if (!IsChecked)
                {
                    _isUncheckfromList = true;
                    Messages.LstQueries[0].IsContentSelected = false;
                }
            }
            if (Messages.LstQueries.Skip(1).All(x => x.IsContentSelected))
            {
                _isUncheckfromList = false;
                Messages.LstQueries[0].IsContentSelected = IsChecked;
            }
            if (_isUncheckfromList)
            {
                _isUncheckfromList = false;
                return;
            }

            if (currentQuery == "All")
            {
                _isUncheckfromList = false;
                Messages.LstQueries.ToList().Select(query => { query.IsContentSelected = IsChecked; return query; }).ToList();
            }
          
            
        }
        private void AddCheckedQueryToList()
        {
            Messages.SelectedQuery.Clear();
            Messages.LstQueries.ToList().ForEach(query =>
            {
                query.Content.Index = Messages.LstQueries.IndexOf(query);
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

        public bool Isupdated { get; set; }



        public ICommand AddMessagesCommand
        {
            get { return (ICommand)GetValue(AddMessagesCommandProperty); }
            set { SetValue(AddMessagesCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddMessagesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddMessagesCommandProperty =
            DependencyProperty.Register("AddMessagesCommand", typeof(ICommand), typeof(MessagesControl));

        private void AddMessagesExecute(object sender)
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
                    if (x.MessageId == Messages.MessageId)
                    {
                        x.MessagesText = Messages.MessagesText;
                        x.LstQueries = Messages.LstQueries;
                    }
                    return x;
                });
                Messages.SelectedQuery.Remove(Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Messages.LstQueries.Select(x => { x.IsContentSelected = false; return x; }).ToList();
                Isupdated = true;

                Dialog.CloseDialog(this);
            }
        }



        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MessagesControl), new PropertyMetadata(OnAvailableItemsChanged));


    }
}
