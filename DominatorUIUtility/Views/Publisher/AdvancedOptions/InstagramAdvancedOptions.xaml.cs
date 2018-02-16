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
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.Publisher.AdvancedOptions
{
    /// <summary>
    /// Interaction logic for InstagramAdvancedOptions.xaml
    /// </summary>
    public partial class InstagramAdvancedOptions : UserControl
    {
        public InstagramAdvancedOptions()
        {
            InitializeComponent();
            AddPosts ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            string LocationDetailFilePath = ConstantVariable.GetConfigurationPath(DominatorHouseCore.Enums.SocialNetworks.Instagram) + "\\LocationsDetail.bin";

            //ObjAddPosts.AddPostViewModel.AddPostModel.LocationDetailsCollection =CollectionViewSource.GetDefaultView(
            //    ProtoBuffBase.DeserializeObjects<LocationDetails>(LocationDetailFilePath));
            MainGrid.DataContext = ObjAddPosts.AddPostViewModel.AddPostModel;
        }
    }
}
