using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for LikerCommentorConfiguration.xaml
    /// </summary>
    public partial class LikerCommentorConfiguration
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LikerCommentorConfigProperty =
            DependencyProperty.Register("LikerCommentorConfig", typeof(LikerCommentorConfigModel),
                typeof(LikerCommentorConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLikeTypeFilterRequiredProperty =
            DependencyProperty.Register("IsLikeTypeFilterRequired", typeof(bool), typeof(LikerCommentorConfiguration),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCommentOptionVisibleProperty =
            DependencyProperty.Register("IsCommentOptionVisible", typeof(bool), typeof(LikerCommentorConfiguration),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLikeOptionVisibleProperty =
            DependencyProperty.Register("IsLikeOptionVisible", typeof(bool), typeof(LikerCommentorConfiguration),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public LikerCommentorConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddCommentsBindCommand = new BaseCommand<object>(sender => true, AddCommentsBindExecute);
        }

        public ICommand AddCommentsBindCommand { get; set; }

        public LikerCommentorConfigModel LikerCommentorConfig
        {
            get => (LikerCommentorConfigModel) GetValue(LikerCommentorConfigProperty);
            set => SetValue(LikerCommentorConfigProperty, value);
        }

        public bool IsLikeTypeFilterRequired
        {
            get => (bool) GetValue(IsLikeTypeFilterRequiredProperty);
            set => SetValue(IsLikeTypeFilterRequiredProperty, value);
        }


        public bool IsCommentOptionVisible
        {
            get => (bool) GetValue(IsCommentOptionVisibleProperty);
            set => SetValue(IsCommentOptionVisibleProperty, value);
        }


        public bool IsLikeOptionVisible
        {
            get => (bool) GetValue(IsLikeOptionVisibleProperty);
            set => SetValue(IsLikeOptionVisibleProperty, value);
        }

        private void AddCommentsBindExecute(object sender)
        {
            //var CommentData = sender as CommentControl;           
            //LikerCommentorConfig.LstManageCommentModel.Add(CommentData.Comments);
            //CommentData.Comments = new ManageCommentModel();
            //LikerCommentorConfig.ManageCommentModel = CommentData.Comments;
            //CommentData.ComboBoxQueries.ItemsSource = LikerCommentorConfig.ManageCommentModel.LstQueries;

            var messageData = sender as CommentControl;

            if (messageData == null) return;

            messageData.Comments.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Comments.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Comments.SelectedQuery.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please select atleast one query!!");
                return;
            }

            if (string.IsNullOrEmpty(messageData.Comments.CommentText))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please enter comment text!!");
                return;
            }


            messageData.Comments.SelectedQuery.Remove(
                messageData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            LikerCommentorConfig.LstManageCommentModel.Add(messageData.Comments);

            messageData.Comments = new ManageCommentModel
            {
                LstQueries = LikerCommentorConfig.ManageCommentModel.LstQueries
            };

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            messageData.Comments.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();

            LikerCommentorConfig.ManageCommentModel = messageData.Comments;

            messageData.ComboBoxQueries.ItemsSource = LikerCommentorConfig.ManageCommentModel.LstQueries;
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Comment_AddCommentToListChanged(object sender, RoutedEventArgs e)
        {
        }

        private void Liketype_checked(object sender, RoutedEventArgs e)
        {
            var currentCheckBoxItem = sender as CheckBox;
            AddRemoveCheckItems(currentCheckBoxItem);
        }

        private void Liketype_Unchecked(object sender, RoutedEventArgs e)
        {
            var currentCheckBoxItem = sender as CheckBox;
            AddRemoveCheckItems(currentCheckBoxItem);
        }


        public void AddRemoveCheckItems(CheckBox content)
        {
            try
            {
                var objReactionType = (ReactionType) Enum.Parse(typeof(ReactionType), content.Name);

                if (content.IsChecked == true)
                    LikerCommentorConfig.ListReactionType.Add(objReactionType);
                else
                    LikerCommentorConfig.ListReactionType.Remove(objReactionType);

                LikerCommentorConfig.ListReactionType = LikerCommentorConfig.ListReactionType.Distinct().ToList();
            }
            catch (Exception)
            {
            }
        }

        private void CommentorConfigControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}