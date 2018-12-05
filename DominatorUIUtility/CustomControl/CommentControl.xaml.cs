using DominatorHouseCore.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;
using DominatorHouseCore.Command;

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
            AddCommentsCommand = new BaseCommand<object>((sender) => true, AddCommentsExecute);

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
            if (string.IsNullOrEmpty(Comments.CommentText))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please type some comment !!");
                return;
            }
              
            AddCheckedQueryToList();
            if (btnAddCommentToList.Content.ToString() == "Update Comment")
            {
                LstManageCommentModel.Select(x =>
                {
                    if (x.CommentId == Comments.CommentId)
                    {
                        x.CommentText = Comments.CommentText;
                        x.FilterText = Comments.FilterText;
                        x.LstQueries = Comments.LstQueries;
                    }
                    return x;
                });
                Comments.SelectedQuery.Remove(Comments.SelectedQuery.FirstOrDefault(x=>x.Content.QueryValue=="All"));
                Comments.LstQueries.Select(x => { x.IsContentSelected = false; return x; }).ToList();
                Isupdated = true;
                Dialog.CloseDialog(this);
            }
            else
            {
                AddCommentToListEventHandler();
            }

           
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
               CheckUncheckAll(sender,true);
        }
       
        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
                CheckUncheckAll(sender,false);
        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            if (((QueryContent)(sender as CheckBox).DataContext).Content.QueryValue == "All")
            {
                Comments.LstQueries.ToList().Select(query => { query.IsContentSelected = IsChecked; return query; }).ToList();
            }

        }
        private void AddCheckedQueryToList()
        {
            Comments.SelectedQuery.Clear();
            Comments.LstQueries.ToList().ForEach(query =>
            {
                    if (query.IsContentSelected)
                    Comments.SelectedQuery.Add(query);
            });
        }
        public bool Isupdated { get; set; } = false;

        public ICommand AddCommentsCommand
        {
            get { return (ICommand)GetValue(AddCommentsCommandProperty); }
            set { SetValue(AddCommentsCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCommentsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommentsCommandProperty =
            DependencyProperty.Register("AddCommentsCommand", typeof(ICommand), typeof(CommentControl));

        private void AddCommentsExecute(object sender)
        {
            if (string.IsNullOrEmpty(Comments.CommentText))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please type some comment !!");
                return;
            }

            AddCheckedQueryToList();
            if (btnAddCommentToList.Content.ToString() == "Update Comment")
            {
                LstManageCommentModel.Select(x =>
                {
                    if (x.CommentId == Comments.CommentId)
                    {
                        x.CommentText = Comments.CommentText;
                        x.FilterText = Comments.FilterText;
                        x.LstQueries = Comments.LstQueries;
                    }
                    return x;
                }).ToList();
                Comments.SelectedQuery.Remove(Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Comments.LstQueries.Select(x => { x.IsContentSelected = false; return x; }).ToList();
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
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommentControl), new PropertyMetadata(OnAvailableItemsChanged));
    }
}
