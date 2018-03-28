using DominatorHouseCore.Models;
using DominatorUIUtility.Behaviours;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            catch (Exception ex)
            {

            }
        }

        private void EditComment_Click(object sender, RoutedEventArgs e)
        {
            var currentItem= ((FrameworkElement)sender).DataContext as ManageCommentModel;
            var EditComment = new CommentControl();
            EditComment.btnAddCommentToList.Content = "Update Comment";
            EditComment.Comments = currentItem;
          
            EditComment.Comments.LstQueries.ToList().ForEach(x =>
            {
                if (EditComment.Comments.SelectedQuery.Contains(x))
                    x.IsContentSelected = true;
            });
            EditComment.MainGrid.Margin =new Thickness(20);
            Dialog dialog = new Dialog();
           Window window= dialog.GetMetroWindow(EditComment, "Edit comment");
            window.Show();
        }

        private void DeleteSingleComment_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as ManageCommentModel;
            LstManageCommentModel.Remove(currentItem);
        }
    }
}
