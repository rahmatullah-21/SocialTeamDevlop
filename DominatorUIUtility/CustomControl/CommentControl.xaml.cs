using DominatorHouseCore.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for CommentControl.xaml
    /// </summary>
    public partial class CommentControl : UserControl
    {
        public CommentControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            
        }


        private static readonly RoutedEvent AddCommentToListEvent =
       EventManager.RegisterRoutedEvent("AddCommentToListChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
           typeof(CommentControl));

        public event RoutedEventHandler AddCommentToListChanged
        {
            add { AddHandler(AddCommentToListEvent, value); }
            remove { RemoveHandler(AddCommentToListEvent, value); }
        }

        void AddCommentToListEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddCommentToListEvent);
            RaiseEvent(routedEventArgs);
        }


        public ManageCommentModel Comments
        {
            get { return (ManageCommentModel)GetValue(ManageCommentsProperty); }
            set { SetValue(ManageCommentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageCommentsProperty =
            DependencyProperty.Register("Comments", typeof(ManageCommentModel), typeof(CommentControl), new PropertyMetadata(OnAvailableItemsChanged));
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get { return (ObservableCollection<ManageCommentModel>)GetValue(LstManageCommentModelProperty); }
            set { SetValue(LstManageCommentModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageCommentModelProperty =
            DependencyProperty.Register("LstManageCommentModel", typeof(ObservableCollection<ManageCommentModel>), typeof(CommentControl), new PropertyMetadata(new ObservableCollection<ManageCommentModel>()));

        private void btnAddCommentToList_Click(object sender, RoutedEventArgs e)
        {
            AddCheckedQueryToList();
            if (btnAddCommentToList.Content.ToString() == "Update Comment")
            {
                LstManageCommentModel.Select(x =>
                {
                    if (x.SerialNo == Comments.SerialNo)
                    {
                        x.CommentText = Comments.CommentText;
                        x.FilterText = Comments.FilterText;
                        x.LstQueries = Comments.LstQueries;
                    }
                    return x;
                });
                Comments.SelectedQuery.Remove("All");
                Comments.LstQueries.Select(x => { x.IsContentSelected = false; return x; }).ToList();
                Window.GetWindow(this).Close();
            }
            else
            {
                AddCommentToListEventHandler();
            }

           
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Content.ToString() == "All")
            {
                CheckUncheckAll(true);
            }

        }

        private void CheckUncheckAll(bool IsChecked)
        {
            Comments.LstQueries.ToList().Select(query => { query.IsContentSelected = IsChecked; return query; }).ToList();
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Content.ToString() == "All")
            {
                CheckUncheckAll(false);
            }
        }
        private void AddCheckedQueryToList()
        {
            Comments.SelectedQuery.Clear();
            Comments.LstQueries.ToList().ForEach(query =>
            {
                    if (query.IsContentSelected)
                    Comments.SelectedQuery.Add(query.Content);
            });
        }
      
    }
}
