using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for MessageMediaControl.xaml
    /// </summary>
    public partial class MessageMediaControl
    {
        public bool Isupdated { get; set; }
        public MessageMediaControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessagesExecute);

        }
        private static readonly RoutedEvent AddMessagesToListEvent =
                EventManager.RegisterRoutedEvent("AddMessagesToListChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
      typeof(MessageMediaControl));

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
            DependencyProperty.Register("Messages", typeof(ManageMessagesModel), typeof(MessageMediaControl), new PropertyMetadata());
        
        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get { return (ObservableCollection<ManageMessagesModel>)GetValue(LstManageMessagesModelProperty); }
            set { SetValue(LstManageMessagesModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>), typeof(MessageMediaControl), new PropertyMetadata(new ObservableCollection<ManageMessagesModel>()));

        public SocialNetworks Network
        {
            get { return (SocialNetworks)GetValue(NetworkProperty); }
            set { SetValue(NetworkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Network.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NetworkProperty =
            DependencyProperty.Register("Network", typeof(SocialNetworks), typeof(MessageMediaControl), new PropertyMetadata(SocialNetworks.Social));
        
        private bool _isUncheckfromList;
        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            var dataContext = (sender as CheckBox)?.DataContext;
            if (dataContext is QueryContent)
            {
                var currentQuery = ((QueryContent) dataContext).Content.QueryValue;
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

                if (currentQuery == "All" || currentQuery == "Default")
                {
                    _isUncheckfromList = false;
                    Messages.LstQueries.ToList().Select(query =>
                    {
                        query.IsContentSelected = IsChecked;
                        return query;
                    }).ToList();
                }
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

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.Filter = "Image Files |*.jpg;*.jpeg;*.png;*.gif";//"Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value)
                {
                    Messages.MediaPath = opf.FileName;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Messages.MediaPath = "";
        }

        public ICommand AddMessagesCommand
        {
            get { return (ICommand)GetValue(AddMessagesCommandProperty); }
            set { SetValue(AddMessagesCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddMessagesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddMessagesCommandProperty =
            DependencyProperty.Register("AddMessagesCommand", typeof(ICommand), typeof(MessageMediaControl));

        private void AddMessagesExecute(object sender)
        {
            if (string.IsNullOrEmpty(Messages.MessagesText) && string.IsNullOrEmpty(Messages.MediaPath))
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
            if (!Messages.LstQueries.Any(x => x.IsContentSelected))
            {
                Dialog.ShowDialog("Warning", "Please select atleast one query.");
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
                        x.MediaPath = Messages.MediaPath;
                        x.SelectedQuery = Messages.SelectedQuery;
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

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MessageMediaControl), new PropertyMetadata());



        private void btnVideos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.Filter = "Video Files |*.mp4;";//"Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value)
                {
                    Messages.MediaPath = opf.FileName;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
