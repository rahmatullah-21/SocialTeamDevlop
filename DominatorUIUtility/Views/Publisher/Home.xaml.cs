using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private Home()
        {
            InitializeComponent();
           
            PublisherDetailCollection = CollectionViewSource.GetDefaultView(PostFileManager.GetAllPost());

            publisherDetail.ItemsSource = PublisherDetailCollection;
        }

        public ICollectionView PublisherDetailCollection { get; set; }


        static Home ObjHome { get; set; } = null;

        public static Home GetSingletonHome()
        {
            return ObjHome ?? (ObjHome = new Home());
        }

        private void btnCreateCampaign_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //createCampign.Visibility = Visibility.Visible;
            //publisherDetail.Visibility = Visibility.Collapsed;
            //publisherPageButtons.Visibility = Visibility.Collapsed;

            PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = new Campaigns();

        }

        private void btnManageDestination_Click(object sender, RoutedEventArgs e) 
            => PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = new ManageDestination();

        private void btnManagePosts_Click(object sender, RoutedEventArgs e)
        {

            var managePosts = new ManagePosts();
            AddPostViewModel ObjAddPostViewModel = new AddPostViewModel();
            managePosts.MainGrid.DataContext = ObjAddPostViewModel.AddPostModel;
            PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = new ManagePosts();

        }
    }
}
