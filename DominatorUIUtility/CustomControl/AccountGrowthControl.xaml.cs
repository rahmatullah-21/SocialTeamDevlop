using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Factories;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountGrowthControl.xaml
    /// </summary>
    public partial class AccountGrowthControl : UserControl, INotifyPropertyChanged
    {
        private DominatorAccountViewModel _dominatorAccountViewModel;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private static SocialNetworks socialNetworks;

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
        

       
            private AccountGrowthControl(DominatorAccountViewModel.AccessorStrategies strategyPack)
        {
            
            _dominatorAccountViewModel = new DominatorAccountViewModel(strategyPack);
            InitializeComponent();
            DominatorAccountViewModel.AccountCollectionView =
                CollectionViewSource.GetDefaultView(DominatorAccountViewModel.LstDominatorAccountModel);
            AccountModule.DataContext = DominatorAccountViewModel;
            DominatorAccountViewModel.PropertyChanged += DominatorAccountViewModel_PropertyChanged;
            worker.DoWork += ReloadGridWithGrowth;
            worker.RunWorkerAsync();

        }

        private void ReloadGridWithGrowth(object sender, DoWorkEventArgs e)
        {
            _accountGrowthInstance.GetRespectiveAccounts(socialNetworks, GrowthPeriod.Daily);
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

        private static AccountGrowthControl _accountGrowthInstance = null;
        
        public static AccountGrowthControl GetAccountGrowthControl(SocialNetworks socialNetwork, DominatorAccountViewModel.AccessorStrategies strategies)
        {
            socialNetworks = socialNetwork;
            if (_accountGrowthInstance == null) 
            {
                _accountGrowthInstance = new AccountGrowthControl(strategies);
            }

            _accountGrowthInstance.GetRespectiveAccounts(socialNetworks);

            return _accountGrowthInstance;
        }


        public static AccountGrowthControl GetAccountGrowthControl(DominatorAccountViewModel.AccessorStrategies strategies)
        {
            return _accountGrowthInstance ?? (_accountGrowthInstance = new AccountGrowthControl(strategies));
        }
        public static AccountGrowthControl GetAccountGrowthControl(SocialNetworks socialNework)
        {
            socialNetworks = socialNework;
            return _accountGrowthInstance;
        }

        public void GetRespectiveAccounts(SocialNetworks socialNetworks, GrowthPeriod period = GrowthPeriod.NoPeriod)
        {

            


            var listCollection = (ListCollectionView)DominatorAccountViewModel.AccountCollectionView;
            if (period == GrowthPeriod.NoPeriod)
            {
                DominatorAccountViewModel.LstDominatorAccountModel.Select(x =>
                {
                    x.IsAccountManagerAccountSelected = false;
                    x.DisplayColumnValue6 = 0;
                    x.DisplayColumnValue7 = 0;
                    x.DisplayColumnValue8 = 0;
                    x.DisplayColumnValue9 = 0;
                    x.DisplayColumnValue10 = 0;
                    return x;
                }).ToList();

                listCollection.Filter = x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == socialNetworks;
            }
            else
            {
                DominatorAccountViewModel.LstDominatorAccountModel.Select(x =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                       .GetSocialLibrary(x.AccountBaseModel.AccountNetwork)
                       .GetNetworkCoreFactory().AccountUpdateFactory;
                    x.IsAccountManagerAccountSelected = false;
                  
                    var AccoutGrowth = accountUpdateFactory.GetDailyGrowth(x.AccountId, x.AccountBaseModel.ProfileId, period);
                    x.DisplayColumnValue6 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue1 : 0;
                    x.DisplayColumnValue7 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue2 : 0;
                    x.DisplayColumnValue8 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue3 : 0;
                    x.DisplayColumnValue9 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue4 : 0;
                    x.DisplayColumnValue10 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue5 : 0;
                    return x;
                }).ToList();

                this.Dispatcher.Invoke(() =>
                {
                    listCollection.Filter = x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == socialNetworks;

                });
            }

            if (socialNetworks == SocialNetworks.Social)
                listCollection.Filter = null;

            var spec = (socialNetworks == SocialNetworks.Social) ?
               DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider() :
               SocinatorInitialize.GetSocialLibrary(socialNetworks)
                     .GetNetworkCoreFactory()
                     .AccountCountFactory.GetColumnSpecificationProvider();



            DominatorAccountViewModel.SocialNetwork = socialNetworks;

        }










        //private void Row_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        //{
        //    List<string> menuOptions = new List<string>();

        //    ListViewItem sourceRow = sender as ListViewItem;

        //    var dominatorAccountModelSelected = ((FrameworkElement)sourceRow)?.DataContext as DominatorAccountModel;

        //    if (sourceRow != null)
        //    {
        //        sourceRow.ContextMenu = new ContextMenu();

        //        if (dominatorAccountModelSelected != null)
        //        {
        //            sourceRow.ContextMenu.ItemsSource = GetContextMenuItems(dominatorAccountModelSelected.AccountBaseModel.AccountNetwork.ToString(), dominatorAccountModelSelected);
        //        }

        //        if (sourceRow.ContextMenu.Items.Count > 0)
        //        {
        //            sourceRow.ContextMenu.PlacementTarget = this;
        //            sourceRow.ContextMenu.IsOpen = true;
        //        }
        //        else
        //        {
        //            sourceRow.ContextMenu = null;
        //        }
        //    }
        //}

        //private IEnumerable<MenuItem> GetContextMenuItems(string socialNetwork, DominatorAccountModel dominatorAccountModel)
        //{
        //    var menuOptions = new List<MenuItem>();

        //    var editProfileMenu = new MenuItem { Header = "Edit Profile" };
        //    editProfileMenu.Click += EditProfile;
        //    var icon = new Image
        //    {
        //        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //        Width = 25,
        //        Height = 25
        //    };
        //    editProfileMenu.DataContext = dominatorAccountModel;
        //    editProfileMenu.Icon = icon;
        //    menuOptions.Add(editProfileMenu);


        //    var deleteProfileMenu = new MenuItem { Header = "Delete Profile" };
        //    deleteProfileMenu.DataContext = dominatorAccountModel;
        //    deleteProfileMenu.Icon = new Image
        //    {
        //        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //        Width = 25,
        //        Height = 25
        //    };
        //    menuOptions.Add(deleteProfileMenu);

        //    var browserLoginMenu = new MenuItem { Header = "Browser Login" };
        //    browserLoginMenu.Click += BrowserLogin;
        //    browserLoginMenu.DataContext = dominatorAccountModel;
        //    browserLoginMenu.Icon = new Image
        //    {
        //        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //        Width = 25,
        //        Height = 25
        //    };
        //    menuOptions.Add(browserLoginMenu);

        //    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
        //    {
        //        var goToToolsMenu = new MenuItem { Header = "Go to Tools" };
        //        goToToolsMenu.Click += GotoTools;
        //        goToToolsMenu.DataContext = dominatorAccountModel;

        //        goToToolsMenu.Icon = new Image
        //        {
        //            Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //            Width = 25,
        //            Height = 25
        //        };
        //        menuOptions.Add(goToToolsMenu);
        //    }

        


        //    var loginStatusMenu = new MenuItem { Header = "Check Account Status" };
        //    loginStatusMenu.Click += CheckinStatus;
        //    loginStatusMenu.DataContext = dominatorAccountModel;
        //    loginStatusMenu.Icon = new Image
        //    {
        //        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //        Width = 25,
        //        Height = 25
        //    };
        //    menuOptions.Add(loginStatusMenu);


        //    var updateMenu = new MenuItem { Header = "Update Friendship" };
        //    updateMenu.Click += UpdateFriendshipCount;
        //    updateMenu.DataContext = dominatorAccountModel;
        //    updateMenu.Icon = new Image
        //    {
        //        Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //        Width = 25,
        //        Height = 25
        //    };
        //    menuOptions.Add(updateMenu);

        //    //
        //    switch (socialNetwork)
        //    {
        //        case "Facebook":
        //            var removePhoneVerificationMenu = new MenuItem { Header = "Remove Phone Verification" };
        //            removePhoneVerificationMenu.Click += FacebookRemovePhoneVerification;
        //            removePhoneVerificationMenu.DataContext = dominatorAccountModel;
        //            removePhoneVerificationMenu.Icon = new Image
        //            {
        //                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //                Width = 25,
        //                Height = 25
        //            };
        //            menuOptions.Add(removePhoneVerificationMenu);
        //            break;

        //        case "Instagram":

        //            var editInstaProfileMenu = new MenuItem { Header = "Edit Insta Profile" };
        //            editInstaProfileMenu.Click += EditInstaProfile;
        //            editInstaProfileMenu.DataContext = dominatorAccountModel;
        //            editInstaProfileMenu.Icon = new Image
        //            {
        //                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //                Width = 25,
        //                Height = 25
        //            };
        //            menuOptions.Add(editInstaProfileMenu);

        //            var phoneVerificationMenu = new MenuItem { Header = "Phone Verification" };
        //            phoneVerificationMenu.Click += InstaPhoneVerification;
        //            phoneVerificationMenu.DataContext = dominatorAccountModel;
        //            phoneVerificationMenu.Icon = new Image
        //            {
        //                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
        //                Width = 25,
        //                Height = 25
        //            };
        //            menuOptions.Add(phoneVerificationMenu);
        //            break;
        //    }

        //    return menuOptions;
        //}

        //public void EditProfile(object sender, RoutedEventArgs e)
        //{
        //    var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

        //    if (dataContext != null) DominatorAccountViewModel.EditAccount(sender);
        //}

       

        //public void GotoTools(object sender, RoutedEventArgs e)
        //{
        //    var dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

        //    if (dominatorAccountModel == null)
        //        return;

        //    DominatorHouseCore.Utility.TabSwitcher.ChangeTabWithNetwork(2, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);
        //}

        //public void BrowserLogin(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
        //        DominatorAccountViewModel.AccountBrowserLogin(dominatorAccountModel);
        //    }
        //    catch (Exception exception)
        //    {
        //        GlobusLogHelper.log.Error(exception.Message);
        //        //MessageBox.Show(exception.Message);
        //        Console.WriteLine(exception);
        //    }
        //}



        //public void CheckinStatus(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
        //        DominatorAccountViewModel.ActionCheckAccount(dominatorAccountModel);
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception);
        //    }
        //}

        //public void UpdateFriendshipCount(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
        //        DominatorAccountViewModel.ActionUpdateAccount(dominatorAccountModel);
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception);
        //    }
        //}

        //public void EditInstaProfile(object sender, RoutedEventArgs e)
        //{

        //}

        //public void InstaPhoneVerification(object sender, RoutedEventArgs e)
        //{

        //}

        //public void InstaCheckAccount(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
        //        DominatorAccountModel objDominatorAccountModel =
        //            ((FrameworkElement)sender).DataContext as DominatorAccountModel;
        //        DominatorAccountViewModel.ActionCheckAccount(dominatorAccountModel);

        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception);
        //        throw;
        //    }
        //}
        private void CmbboxGrowthPeriod_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                var selectedGrowthPeriod = (GrowthPeriod)cmbGrowthPeriod.SelectedIndex;

                _accountGrowthInstance.GetRespectiveAccounts(socialNetworks, selectedGrowthPeriod);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //public void FacebookRemovePhoneVerification(object sender, RoutedEventArgs e)
        //{

        //}


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}
