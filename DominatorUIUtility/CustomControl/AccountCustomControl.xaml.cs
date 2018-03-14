using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountCustomControl.xaml
    /// </summary>
    public partial class AccountCustomControl : UserControl, INotifyPropertyChanged
    {
        private DominatorAccountViewModel _dominatorAccountViewModel = new DominatorAccountViewModel();

        #region Property

        public DominatorAccountViewModel DominatorAccountViewModel
        {
            get
            {
                return _dominatorAccountViewModel;
            }
            set
            {
                _dominatorAccountViewModel = value;
                OnPropertyChanged(nameof(DominatorAccountViewModel));
            }
        }

        #endregion

        private AccountCustomControl()
        {

            InitializeComponent();
            DominatorAccountViewModel.AccountCollectionView =
                CollectionViewSource.GetDefaultView(DominatorAccountViewModel.LstDominatorAccountModel);
            AccountModule.DataContext = DominatorAccountViewModel;
           
        }

       


        private static AccountCustomControl _accountCustomInstance = null;

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNetworks)
        {
            if (_accountCustomInstance == null)
            {
                _accountCustomInstance = new AccountCustomControl();
            }

            _accountCustomInstance.GetRespectiveAccounts(socialNetworks);

            return _accountCustomInstance;
        }


        public static AccountCustomControl GetAccountCustomControl()
        {
            if (_accountCustomInstance == null)
            {
                _accountCustomInstance = new AccountCustomControl();
            }
            return _accountCustomInstance;
        }

        private void GetRespectiveAccounts(SocialNetworks socialNetworks)
        {
            var listCollection = (ListCollectionView)DominatorAccountViewModel.AccountCollectionView;

            switch (socialNetworks)
            {
                case SocialNetworks.Social:
                    listCollection.Filter = null;
                    DominatorAccountViewModel.GridHeaderColumn1.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn1.Header = "Friendship Count";
                    DominatorAccountViewModel.GridHeaderColumn2.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn3.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn4.HeaderVisible = false;
                    //DominatorAccountViewModel.SocialNetworkEditable = true;
                    //DominatorHouseInitializer.ActiveSocialNetwork = SocialNetworks.Social;
                    break;

                case SocialNetworks.Instagram:
                    listCollection.Filter = new Predicate<object>(x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == SocialNetworks.Instagram);
                    DominatorAccountViewModel.GridHeaderColumn1.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn2.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn2.Header = "Follower Count";
                    DominatorAccountViewModel.GridHeaderColumn3.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn3.Header = "Following Count";
                    DominatorAccountViewModel.GridHeaderColumn4.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn4.Header = "Post Count";
                    DominatorAccountViewModel.SocialNetwork = SocialNetworks.Instagram;
                    //DominatorAccountViewModel.SocialNetworkEditable = false;
                    break;

                case SocialNetworks.Twitter:
                    listCollection.Filter = new Predicate<object>(x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == SocialNetworks.Twitter);
                    DominatorAccountViewModel.GridHeaderColumn1.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn2.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn2.Header = "Follower Count";
                    DominatorAccountViewModel.GridHeaderColumn3.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn3.Header = "Following Count";
                    DominatorAccountViewModel.GridHeaderColumn4.HeaderVisible = false;
                    DominatorAccountViewModel.SocialNetwork = SocialNetworks.Twitter;
                  //  DominatorAccountViewModel.SocialNetworkEditable = false;
                    break;
                case SocialNetworks.Quora:
                    listCollection.Filter = new Predicate<object>(x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == SocialNetworks.Quora);
                    DominatorAccountViewModel.GridHeaderColumn1.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn2.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn2.Header = "Follower Count";
                    DominatorAccountViewModel.GridHeaderColumn3.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn3.Header = "Following Count";
                    DominatorAccountViewModel.GridHeaderColumn4.HeaderVisible = false;
                    DominatorAccountViewModel.SocialNetwork = SocialNetworks.Quora;
                    //DominatorAccountViewModel.SocialNetworkEditable = false;
                    break;
                case SocialNetworks.Facebook:
                    listCollection.Filter = new Predicate<object>(x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == SocialNetworks.Facebook);
                    DominatorAccountViewModel.GridHeaderColumn1.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn2.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn2.Header = "Friends Count";
                    DominatorAccountViewModel.GridHeaderColumn3.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn3.Header = "Groups Count";                
                    DominatorAccountViewModel.GridHeaderColumn4.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn4.Header = "Pages Count";
                    DominatorAccountViewModel.SocialNetwork = SocialNetworks.Facebook;
                    break;
                case SocialNetworks.LinkedIn:
                    listCollection.Filter = new Predicate<object>(x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == SocialNetworks.LinkedIn);
                    DominatorAccountViewModel.GridHeaderColumn1.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn2.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn2.Header = "Connection Count";
                    DominatorAccountViewModel.GridHeaderColumn3.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn3.Header = "Groups Count";
                    DominatorAccountViewModel.GridHeaderColumn4.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn4.Header = "Pages Count";
                    DominatorAccountViewModel.SocialNetwork = SocialNetworks.LinkedIn;
                    break;
                case SocialNetworks.Pinterest:
                    listCollection.Filter = new Predicate<object>(x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest);
                    DominatorAccountViewModel.GridHeaderColumn1.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn2.HeaderVisible = true;
                    DominatorAccountViewModel.GridHeaderColumn2.Header = "Follower Count";
                    DominatorAccountViewModel.GridHeaderColumn3.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn3.Header = "Following Count";
                    DominatorAccountViewModel.GridHeaderColumn4.HeaderVisible = false;
                    DominatorAccountViewModel.GridHeaderColumn4.Header = "Board Count";
                    DominatorAccountViewModel.SocialNetwork = SocialNetworks.Pinterest;
                    break;

            }
        }

        private void MangeblacklistedContextMenu_Click(object sender, RoutedEventArgs e)
        {
            BlacklistUserControl objBlacklistUserControl = new BlacklistUserControl();

            var window = new Window()
            {
                Content = objBlacklistUserControl,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            window.ShowDialog();
        }

        private void MangewhitelistUserContextMenu_Click(object sender, RoutedEventArgs e)
        {
            WhitelistuserControl objWhitelistuserControl = new WhitelistuserControl();

            var window = new Window()
            {
                Content = objWhitelistuserControl,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowDialog();
        }

        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            DominatorAccountViewModel.SelectAccountByGroup(sender);
        }

        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
            DominatorAccountViewModel.SelectAccountByGroup(sender);
        }

        private void MenuCheckAccount_OnClick(object sender, RoutedEventArgs e)
        {
            DominatorAccountModel ObjDominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
            //DominatorAccountViewModel.UpdateAccount(ObjDominatorAccountModel);
        }

        private void Row_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            List<string> menuOptions = new List<string>();

            ListViewItem sourceRow = sender as ListViewItem;

            var dominatorAccountModelSelected = ((FrameworkElement)sourceRow)?.DataContext as DominatorAccountModel;

            if (sourceRow != null)
            {
                sourceRow.ContextMenu = new ContextMenu();

                if (dominatorAccountModelSelected != null)
                {
                    sourceRow.ContextMenu.ItemsSource = GetContextMenuItems(dominatorAccountModelSelected.AccountBaseModel.AccountNetwork.ToString(), dominatorAccountModelSelected);
                }

                if (sourceRow.ContextMenu.Items.Count > 0)
                {
                    sourceRow.ContextMenu.PlacementTarget = this;
                    sourceRow.ContextMenu.IsOpen = true;
                }
                else
                {
                    sourceRow.ContextMenu = null;
                }
            }
        }

        private IEnumerable<MenuItem> GetContextMenuItems(string socialNetwork, DominatorAccountModel dominatorAccountModel)
        {
            var menuOptions = new List<MenuItem>();

            var editProfileMenu = new MenuItem { Header = "Edit Profile" };
            editProfileMenu.Click += EditProfile;
            var icon = new Image
            {
                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                Width = 25,
                Height = 25
            };
            editProfileMenu.DataContext = dominatorAccountModel;
            editProfileMenu.Icon = icon;
            menuOptions.Add(editProfileMenu);


            var deleteProfileMenu = new MenuItem { Header = "Delete Profile" };
            deleteProfileMenu.Click += DeleteAccount;
            deleteProfileMenu.DataContext = dominatorAccountModel;
            deleteProfileMenu.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                Width = 25,
                Height = 25
            };
            menuOptions.Add(deleteProfileMenu);

            var goToToolsMenu = new MenuItem { Header = "Go to Tools" };
            goToToolsMenu.Click += GotoTools;
            goToToolsMenu.DataContext = dominatorAccountModel;
            goToToolsMenu.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                Width = 25,
                Height = 25
            };
            menuOptions.Add(goToToolsMenu);


            var loginStatusMenu = new MenuItem { Header = "Check Account Status" };
            loginStatusMenu.Click += CheckinStatus;
            loginStatusMenu.DataContext = dominatorAccountModel;
            loginStatusMenu.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                Width = 25,
                Height = 25
            };
            menuOptions.Add(loginStatusMenu);

            switch (socialNetwork)
            {
                case "Facebook":
                    var removePhoneVerificationMenu = new MenuItem { Header = "Remove Phone Verification" };
                    removePhoneVerificationMenu.Click += FacebookRemovePhoneVerification;
                    removePhoneVerificationMenu.DataContext = dominatorAccountModel;
                    removePhoneVerificationMenu.Icon = new Image
                    {
                        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                        Width = 25,
                        Height = 25
                    };
                    menuOptions.Add(removePhoneVerificationMenu);
                    break;

                case "Instagram":

                    var editInstaProfileMenu = new MenuItem { Header = "Edit Insta Profile" };
                    editInstaProfileMenu.Click += EditInstaProfile;
                    editInstaProfileMenu.DataContext = dominatorAccountModel;
                    editInstaProfileMenu.Icon = new Image
                    {
                        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                        Width = 25,
                        Height = 25
                    };
                    menuOptions.Add(editInstaProfileMenu);

                    var phoneVerificationMenu = new MenuItem { Header = "Phone Verification" };
                    phoneVerificationMenu.Click += InstaPhoneVerification;
                    phoneVerificationMenu.DataContext = dominatorAccountModel;
                    phoneVerificationMenu.Icon = new Image
                    {
                        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                        Width = 25,
                        Height = 25
                    };
                    menuOptions.Add(phoneVerificationMenu);


                    //var checkAccountStatus = new MenuItem { Header = "Check Account Status" };
                    //checkAccountStatus.Click += InstaCheckAccount;
                    //checkAccountStatus.DataContext = dominatorAccountModel;
                    //checkAccountStatus.Icon = new Image
                    //{
                    //    Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                    //    Width = 25,
                    //    Height = 25
                    //};
                    //menuOptions.Add(checkAccountStatus);

                    break;
            }

            return menuOptions;
        }

        public void EditProfile(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dataContext != null) DominatorAccountViewModel.EditAccount(sender);
        }

        public void DeleteAccount(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dataContext != null)
                DominatorAccountViewModel.DeleteAccountByContextMenu(sender);
        }

        public void GotoTools(object sender, RoutedEventArgs e)
        {
            var dominatorAccountModel = ((FrameworkElement) sender).DataContext as DominatorAccountModel;

            DominatorHouseCore.Utility.TabSwitcher.ChangeTabWithNetwork(2, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);

        }


        public void CheckinStatus(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;             
                DominatorAccountViewModel.action_CheckAccount(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public void EditInstaProfile(object sender, RoutedEventArgs e)
        {

        }

        public void InstaPhoneVerification(object sender, RoutedEventArgs e)
        {

        }

        public void InstaCheckAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel =  ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountModel objDominatorAccountModel =
                    ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.action_CheckAccount(dominatorAccountModel);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public void FacebookRemovePhoneVerification(object sender, RoutedEventArgs e)
        {

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
