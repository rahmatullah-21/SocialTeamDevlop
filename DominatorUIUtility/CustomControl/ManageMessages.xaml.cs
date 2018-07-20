using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ManageMessages.xaml
    /// </summary>
    public partial class ManageMessages : UserControl
    {
        public ManageMessages()
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
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>), typeof(ManageMessages), new PropertyMetadata(OnAvailableItemsChanged));
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
            var currentItem = ((FrameworkElement)sender).DataContext as ManageMessagesModel;
            var editMessage = new MessagesControl();
            editMessage.btnAddMessagesToList.Content = "Update Message";
            editMessage.Messages = currentItem;

            editMessage.Messages.LstQueries.ToList().ForEach(x =>
            {
                if (editMessage.Messages.SelectedQuery.Contains(x))
                    x.IsContentSelected = true;
            });
            editMessage.MainGrid.Margin = new Thickness(20);
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(editMessage, "Edit Message");
            window.Show();
        }

        private void DeleteSingleMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as ManageMessagesModel;
            LstManageMessagesModel.Remove(currentItem);
        }
    }
}
