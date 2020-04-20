using System;
using System.Linq;
using DominatorHouseCore.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl.YoutubeCutomControl
{
    // <summary>
    /// Interaction logic for ManageReportVideosAs.xaml
    /// </summary>
    public partial class ManageReportVideosAs : UserControl
    {
        public ManageReportVideosAs()
        {
            InitializeComponent();
            MainGrid.DataContext = this;

        }
        
        public ObservableCollection<ManageReportVideosContentModel> ListReportDetailsModel
        {
            get { return (ObservableCollection<ManageReportVideosContentModel>)GetValue(ListReportDetailsModelProperty); }
            set { SetValue(ListReportDetailsModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListReportDetailsModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListReportDetailsModelProperty =
            DependencyProperty.Register("ListReportDetailsModel", typeof(ObservableCollection<ManageReportVideosContentModel>), typeof(ManageReportVideosAs), new PropertyMetadata(OnAvailableItemsChanged));

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
            catch{ }
        }
        
        private void EditDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentItem = ((FrameworkElement)sender).DataContext as ManageReportVideosContentModel;
                if (currentItem == null)
                    return;

                var editComment = new VideoReportContentControl(currentItem.ReportOption, currentItem.ReportSubOption)
                {
                    btnAddCommentToList = { Content = "LangKeyUpdate".FromResourceDictionary() },
                    ReportDetails = new ManageReportVideosContentModel
                    {
                        CommentText = currentItem.CommentText,
                        LstQueries = new ObservableCollection<QueryContent>(currentItem.LstQueries),
                        CommentId = currentItem.CommentId,
                        SelectedQuery = new ObservableCollection<QueryContent>(currentItem.SelectedQuery),
                        VideoTimestampPercentage = currentItem.VideoTimestampPercentage
                    },
                    ListReportDetailsModel = ListReportDetailsModel,
                };

                editComment.ReportDetails.LstQueries.ToList().ForEach(x =>
                {
                    x.IsContentSelected = false;
                    if (editComment.ReportDetails.SelectedQuery.Any(y => y.Content.QueryValue == x.Content.QueryValue && y.Content.QueryType == x.Content.QueryType))
                        x.IsContentSelected = true;
                });
                editComment.MainGrid.Margin = new Thickness(20);
                Dialog dialog = new Dialog();
                Window window = dialog.GetMetroWindow(editComment, "LangKeyEdit".FromResourceDictionary());
                window.Closed += (s, evnt) =>
                {
                    if (editComment.Isupdated)
                    {
                        var indexToUpdate = ListReportDetailsModel.IndexOf(currentItem);
                        ListReportDetailsModel[indexToUpdate] = editComment.ReportDetails;
                        ListReportDetailsModel[indexToUpdate].ReportOption = editComment.CmbReportOption.SelectedIndex;
                        ListReportDetailsModel[indexToUpdate].ReportSubOption = editComment.CmbReportSubOption.SelectedIndex;
                    }

                    currentItem.LstQueries.Select(query => query.IsContentSelected = false).ToList();
                };
                window.ShowDialog();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        
        private void DeleteSingleReport_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as ManageReportVideosContentModel;
            ListReportDetailsModel.Remove(currentItem);
        }
    }
}
