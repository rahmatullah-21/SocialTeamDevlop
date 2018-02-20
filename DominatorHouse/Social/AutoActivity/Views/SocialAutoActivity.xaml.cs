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
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouse.Social.AutoActivity.Views
{
    /// <summary>
    /// Interaction logic for SocialAutoActivity.xaml
    /// </summary>
    public partial class SocialAutoActivity : UserControl
    {
        private DominatorAutoActivityViewModel DominatorAutoActivityViewModel { get; set; }

        private SocialAutoActivity()
        {
            DominatorAutoActivityViewModel = DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();
            InitializeComponent();
        }

        private void ClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            DominatorAutoActivityViewModel = DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();
            DominatorAutoActivityViewModel.CallRespectiveView(SocialNetworks.Instagram);
        }

        private static SocialAutoActivity ObjSocialAutoActivity { get; set; } = null;

        public static SocialAutoActivity GetSingletonSocialAutoActivity()
        {
            if (ObjSocialAutoActivity == null)           
                ObjSocialAutoActivity = new SocialAutoActivity();
            
            ObjSocialAutoActivity.SetDataContext();
            return ObjSocialAutoActivity;
        }

        private void SetDataContext()
        {
            SocialActivity.DataContext = DominatorAutoActivityViewModel;
        }

        private void ButtonBase1_OnClick(object sender, RoutedEventArgs e)
        {
            DominatorAutoActivityViewModel = DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();
            DominatorAutoActivityViewModel.CallRespectiveView(SocialNetworks.Twitter);
        }

        private void UserName_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dominatorAccountModel =
                ((FrameworkElement) sender).DataContext as DominatorAccountModel;

            if (dominatorAccountModel == null) return;

            switch (dominatorAccountModel.AccountBaseModel.AccountNetwork)
            {

                case SocialNetworks.Instagram:
                    DominatorAutoActivityViewModel =
                        DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();
                    DominatorAutoActivityViewModel.CallRespectiveView(SocialNetworks.Instagram);
                    break;

                case SocialNetworks.Twitter:
                    DominatorAutoActivityViewModel =
                        DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();
                    DominatorAutoActivityViewModel.CallRespectiveView(SocialNetworks.Twitter);
                    break;
            }
        }
    }
}
