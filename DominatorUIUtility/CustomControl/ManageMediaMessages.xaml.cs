using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ManageMediaMessages.xaml
    /// </summary>
    public partial class ManageMediaMessages : UserControl
    {
        public ManageMediaMessages()
        {
            InitializeComponent();
            MainGrid.DataContext = this;

        }
        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get { return (ObservableCollection<ManageMessagesModel>)GetValue(LstManageMessagesModelProperty); }
            set { SetValue(LstManageMessagesModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>), typeof(ManageMediaMessages), new PropertyMetadata(OnAvailableItemsChanged));
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Button)sender).ContextMenu.DataContext = ((Button)sender).DataContext;
                ((Button)sender).ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void EditMessage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentItem = ((FrameworkElement)sender).DataContext as ManageMessagesModel;
                if (currentItem == null)
                    throw new ArgumentNullException(nameof(currentItem));

                var editMessage = new MessageMediaControl
                {
                    btnAddMessagesToList = { Content = "Update Message" },
                    Messages = new ManageMessagesModel
                    {
                        MessagesText = currentItem.MessagesText,
                        LstQueries = new ObservableCollection<QueryContent>(currentItem.LstQueries),
                        MessageId = currentItem.MessageId,
                        SelectedQuery = new ObservableCollection<QueryContent>(currentItem.SelectedQuery),
                        MediaPath = currentItem.MediaPath
                    },
                    LstManageMessagesModel = this.LstManageMessagesModel
                };


                editMessage.Messages.LstQueries.ToList().ForEach(x =>
                {
                    x.IsContentSelected = false || editMessage.Messages.SelectedQuery.Any(y => y.Content.QueryValue == x.Content.QueryValue && y.Content.QueryType == x.Content.QueryType);
                });

                editMessage.MainGrid.Margin = new Thickness(20);
                Dialog dialog = new Dialog();
                Window window = dialog.GetMetroWindow(editMessage, "Edit Message");
                window.Show();

                window.Closed += (s, evnt) =>
                {
                    if (editMessage.Isupdated)
                    {
                        var indexToUpdate = LstManageMessagesModel.IndexOf(currentItem);
                        LstManageMessagesModel[indexToUpdate] = editMessage.Messages;
                    }
                    currentItem.LstQueries.Select(query => query.IsContentSelected = false).ToList();
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteSingleMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as ManageMessagesModel;
            LstManageMessagesModel.Remove(currentItem);
        }

     
    }
}
