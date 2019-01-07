using System.Windows.Controls;
using DominatorHouseCore.Utility;
using CommonServiceLocator;

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
            string LocationDetailFilePath = ServiceLocator.Current.GetInstance<IConstantVariable>().GetConfigurationDir(DominatorHouseCore.Enums.SocialNetworks.Instagram) + "\\LocationsDetail.bin";

            //ObjAddPosts.AddPostViewModel.AddPostModel.LocationDetailsCollection =CollectionViewSource.GetDefaultView(
            //    ProtoBuffBase.DeserializeObjects<LocationDetails>(LocationDetailFilePath));
            MainGrid.DataContext = ObjAddPosts.AddPostViewModel.AddPostModel;
        }
    }
}
