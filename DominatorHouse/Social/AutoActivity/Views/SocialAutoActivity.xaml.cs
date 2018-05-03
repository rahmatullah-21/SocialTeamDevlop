using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
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
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;

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


        public static bool NewAutoActivityObject(SocialNetworks soicalNetworks,string selectedAccounts)
        {
            try
            {
                ObjSocialAutoActivity = new SocialAutoActivity();
                ObjSocialAutoActivity.SetDataContext();
                ObjSocialAutoActivity.DominatorAutoActivityViewModel.CallRespectiveView(soicalNetworks);
                switch (soicalNetworks)
                {
                    case SocialNetworks.Facebook:
                        SelectedDominatorAccounts.FdAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.Instagram:
                        SelectedDominatorAccounts.GdAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.Twitter:
                        SelectedDominatorAccounts.TdAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.Pinterest:
                        SelectedDominatorAccounts.PdAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.LinkedIn:
                        SelectedDominatorAccounts.LdAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.Reddit:
                        SelectedDominatorAccounts.RdAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.Quora:
                        SelectedDominatorAccounts.QdAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.Gplus:
                        SelectedDominatorAccounts.GplusAccounts = selectedAccounts;
                        break;
                    case SocialNetworks.Youtube:
                        SelectedDominatorAccounts.YdAccounts = selectedAccounts;
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       
        private void SetDataContext()
        {
            SocialActivity.DataContext = DominatorAutoActivityViewModel;
        }


        private void UserName_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var accountsActivityDetailModel =
                ((FrameworkElement) sender).DataContext as AccountsActivityDetailModel;

            var dominatorAccountModel = accountsActivityDetailModel?.DominatorAccountModel;

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

        private void Expander_OnExpanded(object sender, RoutedEventArgs e)
        {
            var data = ((FrameworkElement) sender).DataContext as AccountsActivityDetailModel;
          
            if (data == null || data.AutoActivityModuleDetailsCollections.Count != 0) return;

            var currentExpander = sender as Expander;

            if (currentExpander != null)
              currentExpander.IsExpanded = false;
            GlobusLogHelper.log.Info($"No acvitity details are found {data.DominatorAccountModel.AccountBaseModel.UserName}");
        }

        private void ExpandAll_OnClick(object sender, RoutedEventArgs e)
        {
            var data = ((FrameworkElement)sender).DataContext as DominatorAutoActivityViewModel;

            data?.AccountsCollection.ForEach(x => { x.IsExpand = true; });
        }

        private void MenuShrinkAll_OnClick(object sender, RoutedEventArgs e)
        {
            var data = ((FrameworkElement) sender).DataContext as DominatorAutoActivityViewModel;

            data?.AccountsCollection.ForEach(x => { x.IsExpand = false; });
        }

        private void BtnSelect_OnClick(object sender, RoutedEventArgs e)
        {
            var contextMenu = ((Button) sender).ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.DataContext = ((Button) sender).DataContext;
                contextMenu.IsOpen = true;
            }
        }

        private void SocialAutoActivity_OnLoaded(object sender, RoutedEventArgs e)
        {
            DominatorAutoActivityViewModel.InitializeAccounts();
            SetDataContext();
        }


    }
}
