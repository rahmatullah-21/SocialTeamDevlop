using DominatorHouseCore.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore;
using System;
using System.Collections.Generic;

namespace DominatorUIUtility.CustomControl.YoutubeCutomControl
{
    public partial class VideoReportContentControl : UserControl
    {
        public VideoReportContentControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddCommentsCommand = new BaseCommand<object>((sender) => true, AddCommentsExecute);
            CmbReportOption.ItemsSource = new List<string>() { "Sexual content", "Violent or repulsive content", "Hateful or abusive content", "Harmful dangerous acts", "Child abuse", "Promotes terrorism", "Spam or misleading", "Infringes my rights", "Captions issue" };
        }

        public VideoReportContentControl(int optionIndex, int subOptionIndex) : this()
        {
            CmbReportOption.SelectedIndex = optionIndex;
            CmbReportSubOption.SelectedIndex = subOptionIndex;
        }

        private static readonly RoutedEvent AddCommentToListEvent =
       EventManager.RegisterRoutedEvent("AddCommentToListChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
           typeof(VideoReportContentControl));

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


        public ManageReportVideosContentModel Comments
        {
            get { return (ManageReportVideosContentModel)GetValue(ManageCommentsProperty); }
            set { SetValue(ManageCommentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageCommentsProperty =
            DependencyProperty.Register("Comments", typeof(ManageReportVideosContentModel), typeof(VideoReportContentControl), new PropertyMetadata(OnAvailableItemsChanged));
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        public ObservableCollection<ManageReportVideosContentModel> LstManageCommentModel
        {
            get { return (ObservableCollection<ManageReportVideosContentModel>)GetValue(LstManageCommentModelProperty); }
            set { SetValue(LstManageCommentModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageCommentModelProperty =
            DependencyProperty.Register("LstManageCommentModel", typeof(ObservableCollection<ManageReportVideosContentModel>), typeof(VideoReportContentControl), new PropertyMetadata(new ObservableCollection<ManageReportVideosContentModel>()));

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }
        private bool _isUncheckfromList;
        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            try
            {
                var currentQuery = ((QueryContent)(sender as CheckBox).DataContext).Content.QueryValue;
                if (!Comments.LstQueries.Skip(1).All(x => x.IsContentSelected))
                {
                    if (!IsChecked)
                    {
                        _isUncheckfromList = true;
                        Comments.LstQueries[0].IsContentSelected = false;
                    }
                }
                if (Comments.LstQueries.Skip(1).All(x => x.IsContentSelected))
                {
                    _isUncheckfromList = false;
                    Comments.LstQueries[0].IsContentSelected = IsChecked;
                }
                if (_isUncheckfromList)
                {
                    _isUncheckfromList = false;
                    return;
                }

                if (currentQuery == "All" || currentQuery == "Default")
                {
                    _isUncheckfromList = false;
                    Comments.LstQueries.ToList().Select(query => { query.IsContentSelected = IsChecked; return query; }).ToList();
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
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
        public bool Isupdated { get; set; }

        public ICommand AddCommentsCommand
        {
            get { return (ICommand)GetValue(AddCommentsCommandProperty); }
            set { SetValue(AddCommentsCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCommentsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommentsCommandProperty =
            DependencyProperty.Register("AddCommentsCommand", typeof(ICommand), typeof(VideoReportContentControl));

        private void AddCommentsExecute(object sender)
        {
            if (string.IsNullOrEmpty(Comments.CommentText))
            {
                Dialog.ShowDialog("Warning", "Please type some comment !!");
                return;
            }
            if (!Comments.LstQueries.Any(x => x.IsContentSelected))
            {
                Dialog.ShowDialog("Warning", "Please select atleast one query.");
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
                        x.SelectedQuery = Comments.SelectedQuery;
                        x.ReportOption = CmbReportOption.SelectedIndex;
                        x.ReportSubOption = CmbReportSubOption.SelectedIndex;
                    }
                    return x;
                }).ToList();
                Comments.SelectedQuery.Remove(Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Comments.LstQueries.Select(x => { x.IsContentSelected = false; return x; }).ToList();
                Comments.ReportOption = CmbReportOption.SelectedIndex;
                Comments.ReportSubOption = CmbReportSubOption.SelectedIndex;
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
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(VideoReportContentControl), new PropertyMetadata(OnAvailableItemsChanged));

        private void CmbReportOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var getList = SetSubOptions((sender as ComboBox).SelectedIndex);
                CmbReportSubOption.ItemsSource = getList;
                if (getList != null)
                    CmbReportSubOption.SelectedIndex = 0;
            }
            catch (Exception ex) { ex.DebugLog(); }
        }

        List<string> SetSubOptions(int optionIndex)
        {
            CmbReportSubOption.Visibility = Visibility.Visible;
            var list = new List<string>();
            switch(optionIndex)
            {
                case 0:
                    list = new List<string> { "Graphic sexual activity", "Nudity", "Suggestive, but without nudity", "Content involving minors", "Abusive title or description", "Other sexual content" };
                    break;
                case 1:
                    list = new List<string> { "Adults fighting", "Physical attack", "Youth violence", "Animal abuse" };
                    break;
                case 2:
                    list = new List<string> { "Promotes hatred or violence", "Abusing vulnerable individuals", "Bullying", "Abusive title or description" };
                    break;
                case 3:
                    list = new List<string> { "Pharmaceutical or drug abuse", "Abuse of fire or explosives", "Suicide or self injury", "Other dangerous acts" };
                    break;
                case 4:
                case 5:
                    CmbReportSubOption.Visibility = Visibility.Hidden;
                    list?.Clear();
                    return null;
                case 6:
                    list = new List<string> { "Mass advertising", "Pharmaceutical drugs for sale", "Misleading text", "Misleading thumbnail", "Scams/fraud" };
                    break;
                case 7:
                    list = new List<string> { "Infringes my copyright", "Invades my privacy", "Other legal claim" };
                    break;
                case 8:
                    list = new List<string> { "Captions are missing (CVAA)", "Captions are inaccurate", "Captions are abusive" };
                    break;
                default:
                    break;
            }
            return list;
        }
    }
}
