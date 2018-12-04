using DominatorHouseCore.Models;
using DominatorUIUtility.Behaviours;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ManageComment.xaml
    /// </summary>
    public partial class ManageComment : UserControl
    {
        public ManageComment()
        {
            InitializeComponent();
            MainGrid.DataContext = this;

        }


        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get { return (ObservableCollection<ManageCommentModel>)GetValue(LstManageCommentModelProperty); }
            set { SetValue(LstManageCommentModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageCommentModelProperty =
            DependencyProperty.Register("LstManageCommentModel", typeof(ObservableCollection<ManageCommentModel>), typeof(ManageComment), new PropertyMetadata(OnAvailableItemsChanged));
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
            catch (Exception)
            {

            }
        }

        private void EditComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentItem = ((FrameworkElement)sender).DataContext as ManageCommentModel;
                if (currentItem == null)
                    throw new ArgumentNullException(nameof(currentItem));

                var editComment = new CommentControl
                {
                    btnAddCommentToList = { Content = "Update Comment" },
                    Comments = new ManageCommentModel
                    {
                        CommentText = currentItem.CommentText,
                        LstQueries = new ObservableCollection<QueryContent>(currentItem.LstQueries),
                        CommentId = currentItem.CommentId,
                        SelectedQuery = new ObservableCollection<QueryContent>(currentItem.SelectedQuery),
                        FilterText = currentItem.FilterText
                    },
                    LstManageCommentModel = LstManageCommentModel
                };

                editComment.Comments.LstQueries.ToList().ForEach(x =>
                {
                    x.IsContentSelected = false;
                    if (editComment.Comments.SelectedQuery.Any(y => y.Content.QueryValue == x.Content.QueryValue && y.Content.QueryType == x.Content.QueryType))
                        x.IsContentSelected = true;
                });
                editComment.MainGrid.Margin = new Thickness(20);
                Dialog dialog = new Dialog();
                Window window = dialog.GetMetroWindow(editComment, "Edit comment");
                window.Show();
                window.Closed += (s, evnt) =>
                {
                    if (editComment.Isupdated)
                    {
                        var indexToUpdate = LstManageCommentModel.IndexOf(currentItem);
                        LstManageCommentModel[indexToUpdate] = editComment.Comments;
                    }

                    currentItem.LstQueries.Select(query => query.IsContentSelected = false).ToList();
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteSingleComment_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as ManageCommentModel;
            LstManageCommentModel.Remove(currentItem);
        }
    }
}
