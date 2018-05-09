using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePosts.xaml
    /// </summary>
    public partial class PublisherManagePosts : UserControl
    {
        private PublisherManagePosts()
        {
            InitializeComponent();
            ManagePost.DataContext = PublisherManagePostsViewModel;
        }

        public PublisherManagePostsViewModel PublisherManagePostsViewModel { get; set; } =
            new PublisherManagePostsViewModel();


        private static PublisherManagePosts _instance;

        public static PublisherManagePosts Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePosts());


        private void TabsSelection_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as TabItemTemplates;
            PublisherManagePostsViewModel.SelectedTabs = dataContext?.Title;
        }

        private void ComboCampaignList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
    }
}
