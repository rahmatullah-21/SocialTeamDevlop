using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Models.FacebookModels;

namespace LegionUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    /// Interaction logic for ManageEventDetails.xaml
    /// </summary>
    public partial class ManageEventDetails : UserControl
    {
        public ManageEventDetails()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public ObservableCollection<EventCreaterManagerModel> LstManageEventModel
        {
            get { return (ObservableCollection<EventCreaterManagerModel>)GetValue(LstManageEventModelProperty); }
            set { SetValue(LstManageEventModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageEventModelProperty =
            DependencyProperty.Register("LstManageEventModel", typeof(ObservableCollection<EventCreaterManagerModel>),
                typeof(ManageEventDetails), new PropertyMetadata(OnAvailableItemsChanged));
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public ICommand EditValueCommand
        {
            get { return (ICommand)GetValue(EditValueCommandProperty); }
            set { SetValue(EditValueCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditEventCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditValueCommandProperty =
            DependencyProperty.Register("EditValueCommand", typeof(ICommand), typeof(ManageEventDetails));


        public ICommand DeleteValueCommand
        {
            get { return (ICommand)GetValue(DeleteValueCommandProperty); }
            set { SetValue(DeleteValueCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddEventCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteValueCommandProperty =
            DependencyProperty.Register("DeleteValueCommand", typeof(ICommand), typeof(ManageEventDetails));


        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentItem = ((FrameworkElement)sender).DataContext as EventCreaterManagerModel;
                if (currentItem != null)
                {
                    var editMessage = new EventCreaterManager()
                    {
                        
                        BtnAddEventToList = { Content = "Update Event" },
                        EventCreaterManagerModelCommand = new EventCreaterManagerModel
                        {
                            Id=currentItem.Id,
                            EventType = currentItem.EventType,
                            EventName = currentItem.EventName,
                            EventDescription = currentItem.EventDescription,
                            EventLocation = currentItem.EventLocation,
                            EventStartDate = currentItem.EventStartDate,
                            EventEndDate = currentItem.EventEndDate,
                            IsAnyOneCanPostForAllPost = currentItem.IsAnyOneCanPostForAllPost,
                            IsGuestCanInviteFriends = currentItem.IsGuestCanInviteFriends,
                            IsShowGuestList = currentItem.IsShowGuestList,
                            MediaPath=currentItem.MediaPath,
                            FbMultiMediaModel = currentItem.FbMultiMediaModel,
                            IsPostMustApproved = currentItem.IsPostMustApproved,
                            IsQuesOnMessanger = currentItem.IsQuesOnMessanger,
                            IsSelectLocation = currentItem.IsSelectLocation,
                            IsPrivatePostingVisibile = currentItem.IsPrivatePostingVisibile,
                            IsPublicPostingVisibile = currentItem.IsPublicPostingVisibile,
                            TextLength = currentItem.TextLength,
                            EventId = currentItem.EventId,
                        },
                        LstManageEventModel = LstManageEventModel
                    };

                    editMessage.MainGrid.Margin = new Thickness(20);
                   
                    Dialog dialog = new Dialog();
                    Window window = dialog.GetMetroWindow(editMessage, "Edit Message");
                    window.ShowDialog();
                    window.Closed += (s, evnt) =>
                    {
                        if (editMessage.Isupdated)
                        {
                            var indexToUpdate = LstManageEventModel.IndexOf(currentItem);
                            LstManageEventModel[indexToUpdate] = editMessage.EventCreaterManagerModelCommand;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteSingleMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as EventCreaterManagerModel;

            LstManageEventModel.Remove(currentItem);
        }

        private void BtnAction_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu != null)
                {
                    contextMenu.DataContext = ((Button)sender).DataContext;
                    contextMenu.IsOpen = true;
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}
