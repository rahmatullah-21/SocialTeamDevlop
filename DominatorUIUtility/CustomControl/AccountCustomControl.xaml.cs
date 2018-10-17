using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Factories;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Unity;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountCustomControl.xaml
    /// </summary>
    public partial class AccountCustomControl : UserControl, INotifyPropertyChanged
    {
        private DominatorAccountViewModel _dominatorAccountViewModel;

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
            _dominatorAccountViewModel = (DominatorAccountViewModel)DominatorHouseCore.IoC.Container.Resolve<IDominatorAccountViewModel>();
            InitializeComponent();
            DominatorAccountViewModel.AccountCollectionView =
                CollectionViewSource.GetDefaultView(DominatorAccountViewModel.LstDominatorAccountModel);
            AccountModule.DataContext = DominatorAccountViewModel;
            DominatorAccountViewModel.PropertyChanged += DominatorAccountViewModel_PropertyChanged;
        }

        List<GridViewColumn> _addedColumns = new List<GridViewColumn>();

        private void DominatorAccountViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VisibleColumns")
            {
                // create a new layout correspondingly
                // remove existing columns
                GridView gv = (GridView)AccountListView.View;
                _addedColumns.ForEach(gvc => gv.Columns.Remove(gvc));
                _addedColumns.Clear();

                // add one column for each needed
                var colIndex = 0;
                _addedColumns = _dominatorAccountViewModel.VisibleColumns
                    .Select(name => new GridViewColumn
                    {
                        DisplayMemberBinding = new Binding($"DisplayColumnValue{++colIndex}"),
                        Header = name,
                        Width = 130
                    }).ToList();
                _addedColumns.ForEach(gv.Columns.Add);
            }
        }

        private static AccountCustomControl _accountCustomInstance = null;

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNetworks, AccessorStrategies strategies)
        {
            if (_accountCustomInstance == null)
            {
                _accountCustomInstance = new AccountCustomControl();
            }

            _accountCustomInstance.GetRespectiveAccounts(socialNetworks);

            return _accountCustomInstance;
        }


        public static AccountCustomControl GetAccountCustomControl(AccessorStrategies strategies)
        {
            return _accountCustomInstance ?? (_accountCustomInstance = new AccountCustomControl());
        }
        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNework)
        {
            return _accountCustomInstance ?? (_accountCustomInstance = new AccountCustomControl());
        }

        public void GetRespectiveAccounts(SocialNetworks socialNetworks)
        {
            var listCollection = (ListCollectionView)DominatorAccountViewModel.AccountCollectionView;
            DominatorAccountViewModel.LstDominatorAccountModel.Select(x =>
            {
                x.IsAccountManagerAccountSelected = false;
                return x;
            }).ToList();
            listCollection.Filter = x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == socialNetworks;

            if (socialNetworks == SocialNetworks.Social)
                listCollection.Filter = null;

            var spec = (socialNetworks == SocialNetworks.Social) ?
                DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider() :
                SocinatorInitialize.GetSocialLibrary(socialNetworks)
                      .GetNetworkCoreFactory()
                      .AccountCountFactory.GetColumnSpecificationProvider();
            DominatorAccountViewModel.VisibleColumns = spec.VisibleHeaders;
        }

        private void Row_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
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

            #region Details Menu

            var image = Application.Current.FindResource("appbar_book_open_hardcover");
            var convasImage = GetConvasImage(image);

            var deatilProfileMenu = new MenuItem { Header = "Details", Icon = convasImage };
            deatilProfileMenu.Click += ProfileDetails;
            deatilProfileMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(deatilProfileMenu);

            #endregion

            #region Delete Profile Menu

            image = Application.Current.FindResource("appbar_delete");
            convasImage = GetConvasImage(image);

            var deleteProfileMenu = new MenuItem { Header = "Delete Profile", Icon = convasImage };
            deleteProfileMenu.Click += DeleteAccount;
            deleteProfileMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(deleteProfileMenu);

            #endregion

            #region Browser Login Menu

            image = Application.Current.FindResource("appbar_browser");
            convasImage = GetConvasImage(image);
            var browserLoginMenu = new MenuItem { Header = "Browser Login", Icon = convasImage };
            browserLoginMenu.Click += BrowserLogin;
            browserLoginMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(browserLoginMenu);

            #endregion

            #region Go to Tools Menu

            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                image = Application.Current.FindResource("appbar_tools");
                convasImage = GetConvasImage(image);

                var goToToolsMenu = new MenuItem { Header = "Go to Tools", Icon = convasImage };
                goToToolsMenu.Click += GotoTools;
                goToToolsMenu.DataContext = dominatorAccountModel;
                menuOptions.Add(goToToolsMenu);
            }

            #endregion

            #region Check Account Status Menu

            image = Application.Current.FindResource("appbar_page_search");
            convasImage = GetConvasImage(image);

            var loginStatusMenu = new MenuItem { Header = "Check Account Status", Icon = convasImage };
            loginStatusMenu.Click += CheckinStatus;
            loginStatusMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(loginStatusMenu);

            #endregion

            #region Update Friendship Menu

            image = Application.Current.FindResource("appbar_group");
            convasImage = GetConvasImage(image);

            var updateMenu = new MenuItem { Header = "Update Friendship", Icon = convasImage };
            updateMenu.Click += UpdateFriendshipCount;
            updateMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(updateMenu);

            #endregion

            switch (socialNetwork)
            {
                case "Facebook":
                    break;

                case "Instagram":

                    #region Edit Insta Profile Menu

                    image = Application.Current.FindResource("appbar_page_edit");
                    convasImage = GetConvasImage(image);

                    var editInstaProfileMenu = new MenuItem { Header = "Edit Insta Profile", Icon = convasImage };
                    editInstaProfileMenu.Click += EditNetworkProfile;
                    editInstaProfileMenu.DataContext = dominatorAccountModel;
                    menuOptions.Add(editInstaProfileMenu);

                    #endregion

                    break;
                case "Twitter":

                    #region Edit Twitter Profile Menu

                    image = Application.Current.FindResource("appbar_page_edit");
                    convasImage = GetConvasImage(image);

                    var editTwtProfileMenu = new MenuItem { Header = "Edit Twitter Profile", Icon = convasImage };
                    editTwtProfileMenu.Click += EditNetworkProfile;
                    editTwtProfileMenu.DataContext = dominatorAccountModel;
                    menuOptions.Add(editTwtProfileMenu);

                    #endregion
                    break;
            }
            #region Edit Twitter Profile Menu

            image = Application.Current.FindResource("appbar_page_duplicate");
            convasImage = GetConvasImage(image);

            var copyAccountId = new MenuItem { Header = "Copy Account Id", Icon = convasImage };
            copyAccountId.Click += CopyAccountId;
            copyAccountId.DataContext = dominatorAccountModel;
            menuOptions.Add(copyAccountId);

            #endregion
            return menuOptions;
        }

        private void CopyAccountId(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
            if (!string.IsNullOrEmpty(dataContext.AccountId))
            {
                Clipboard.SetText(dataContext.AccountId);
                ToasterNotification.ShowSuccess("AccountId copied");
            }

        }

        private static Rectangle GetConvasImage(object image)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 18;
            rectangle.Height = 20;
            rectangle.Fill = Brushes.Black;
            VisualBrush visualBrush = new VisualBrush();
            visualBrush.Visual = image as Visual;
            visualBrush.Stretch = Stretch.Fill;
            rectangle.OpacityMask = visualBrush;
            return rectangle;
        }

        private void ProfileDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                AccountManager.GetSingletonAccountManager(String.Empty, dataContext, dataContext.AccountBaseModel.AccountNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
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
            AccountListView.ItemsSource = DominatorAccountViewModel.AccountCollectionView;

        }

        public void GotoTools(object sender, RoutedEventArgs e)
        {
            var dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dominatorAccountModel == null)
                return;

            DominatorHouseCore.Utility.TabSwitcher.ChangeTabWithNetwork(3, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);
        }

        public void BrowserLogin(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.AccountBrowserLogin(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                GlobusLogHelper.log.Error(exception.Message);
                //MessageBox.Show(exception.Message);
                Console.WriteLine(exception);
            }
        }



        public void CheckinStatus(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.ActionCheckAccount(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void UpdateFriendshipCount(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.ActionUpdateAccount(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void EditNetworkProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

                DominatorAccountViewModel.EditProfile(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void InstaPhoneVerification(object sender, RoutedEventArgs e)
        {

        }

        public void InstaCheckAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountModel objDominatorAccountModel =
                    ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.ActionCheckAccount(dominatorAccountModel);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public void FacebookRemovePhoneVerification(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.RemovePhoneVerification(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
