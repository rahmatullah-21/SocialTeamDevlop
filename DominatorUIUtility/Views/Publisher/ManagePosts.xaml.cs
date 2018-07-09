using System;
using System.Collections.Generic;
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
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for ManagePosts.xaml
    /// </summary>
    public partial class ManagePosts : UserControl
    {
        public AddPostViewModel ObjAddPostViewModel = new AddPostViewModel();
        public ManagePosts()
        {
            InitializeComponent();
            publishersHeader.HeaderText = FindResource("LangKeyManagePosts").ToString();
            MainGrid.DataContext = ObjAddPostViewModel.AddPostModel;
            ObjAddPostViewModel.AddPostModel.PostsDetailCollection = CollectionViewSource.GetDefaultView(PostFileManager.GetAllPost());
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Continue");
        }
    }
}
