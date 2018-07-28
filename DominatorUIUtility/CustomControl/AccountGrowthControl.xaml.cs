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
using LiveCharts;
using LiveCharts.Wpf;
using System.Reflection;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountGrowthControl.xaml
    /// </summary>
    public partial class AccountGrowthControl : UserControl, INotifyPropertyChanged
    {
        private DominatorAccountViewModel _dominatorAccountViewModel;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly BackgroundWorker chartWorker = new BackgroundWorker();

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
            InitializeChart();


            worker.DoWork += ReloadGridWithGrowth;

            worker.RunWorkerAsync();




        }

        private void InitializeChart()
        {
            DominatorAccountViewModel.GrowthChartProperties = new List<string>() { "Followers", "Followings", "Tweets" };
            DominatorAccountViewModel.GrowthChartPeriods = GetChartPeriodEnumStringList();
            DominatorAccountViewModel.GrowthChartTypes = new List<string>() { "Gain", "Total", "Both" };
            DominatorAccountViewModel.GrowthChartAccountNumber = DominatorAccountViewModel.LstDominatorAccountModel[0].AccountId;
            DominatorAccountViewModel.GrowthChartPeriod = "Past 30 days";
            DominatorAccountViewModel.GrowthChartProperty = "Followers";
            DominatorAccountViewModel.GrowthChartType = "Total";
            DominatorAccountViewModel.SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Followers",
                    Values = new ChartValues<double> {0,1,3,4,5, 6,7,8,11,10,20,19,19,19,19,19,19,19,20,20,21,21,24,24,24,24,24,28,29,29 }
                }

            };

        }




        private void CartesianChart1OnDataClick(object sender, ChartPoint chartPoint)
        {
            MessageBox.Show("You clicked (" + chartPoint.X + "," + chartPoint.Y + ")");
        }
        private void ReloadGridWithGrowth(object sender, DoWorkEventArgs e)
        {
            _accountGrowthInstance.GetRespectiveAccounts(SocialNetworks.Twitter, GrowthPeriod.Daily);
            PlotChart();

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

        public static AccountGrowthControl GetAccountGrowthControl(SocialNetworks socialNetworks, DominatorAccountViewModel.AccessorStrategies strategies)
        {
            if (_accountGrowthInstance == null)
            {
                _accountGrowthInstance = new AccountGrowthControl(strategies);
            }

            _accountGrowthInstance.GetRespectiveAccounts(socialNetworks);

            return _accountGrowthInstance;
        }

        public List<string> GetChartPeriodEnumStringList()
        {
            var list = new List<string>();
            foreach (GrowthChartPeriod period in Enum.GetValues(typeof(GrowthChartPeriod)))
            {
                list.Add(StringValueOfEnum(period));
            }
            return list;
        }
        static string StringValueOfEnum(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static AccountGrowthControl GetAccountGrowthControl(DominatorAccountViewModel.AccessorStrategies strategies)
        {
            return _accountGrowthInstance ?? (_accountGrowthInstance = new AccountGrowthControl(strategies));
        }
        public static AccountGrowthControl GetAccountGrowthControl(SocialNetworks socialNework)
        {
            return _accountGrowthInstance;
        }

        private void GetRespectiveAccounts(SocialNetworks socialNetworks, GrowthPeriod period = GrowthPeriod.NoPeriod)
        {

            var accountUpdateFactory = SocinatorInitialize
                       .GetSocialLibrary(SocialNetworks.Twitter)
                       .GetNetworkCoreFactory().AccountUpdateFactory;


            var listCollection = (ListCollectionView)DominatorAccountViewModel.AccountCollectionView;
            if (period == GrowthPeriod.NoPeriod)
            {
                DominatorAccountViewModel.LstDominatorAccountModel.Select(x =>
                {
                    x.IsAccountManagerAccountSelected = false;
                    x.DisplayColumnValue4 = 0;
                    x.DisplayColumnValue5 = 0;
                    x.DisplayColumnValue6 = 0;
                    return x;
                }).ToList();

                listCollection.Filter = x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == socialNetworks;
            }
            else
            {
                DominatorAccountViewModel.LstDominatorAccountModel.Select(x =>
                {
                    x.IsAccountManagerAccountSelected = false;
                    var AccoutGrowth = accountUpdateFactory.GetDailyGrowth(x.AccountId, x.AccountBaseModel.ProfileId, period);
                    x.DisplayColumnValue4 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue1 : 0;
                    x.DisplayColumnValue5 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue2 : 0;
                    x.DisplayColumnValue6 = AccoutGrowth != null ? AccoutGrowth.GrowthColumnValue3 : 0;


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
            deleteProfileMenu.DataContext = dominatorAccountModel;
            deleteProfileMenu.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                Width = 25,
                Height = 25
            };
            menuOptions.Add(deleteProfileMenu);

            var browserLoginMenu = new MenuItem { Header = "Browser Login" };
            browserLoginMenu.Click += BrowserLogin;
            browserLoginMenu.DataContext = dominatorAccountModel;
            browserLoginMenu.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                Width = 25,
                Height = 25
            };
            menuOptions.Add(browserLoginMenu);

            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
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
            }




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


            var updateMenu = new MenuItem { Header = "Update Friendship" };
            updateMenu.Click += UpdateFriendshipCount;
            updateMenu.DataContext = dominatorAccountModel;
            updateMenu.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/DominatorUIUtility;component/Images/setting.png", UriKind.Relative)),
                Width = 25,
                Height = 25
            };
            menuOptions.Add(updateMenu);

            //
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
                    break;
            }

            return menuOptions;
        }

        public void EditProfile(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dataContext != null) DominatorAccountViewModel.EditAccount(sender);
        }



        public void GotoTools(object sender, RoutedEventArgs e)
        {
            var dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dominatorAccountModel == null)
                return;

            DominatorHouseCore.Utility.TabSwitcher.ChangeTabWithNetwork(2, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);
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
        private void CmbboxGrowthPeriod_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                var selectedGrowthPeriod = (GrowthPeriod)cmbGrowthPeriod.SelectedIndex;
                _accountGrowthInstance.GetRespectiveAccounts(SocialNetworks.Twitter, selectedGrowthPeriod);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void CmbboxGrowthChartAccount_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {

                GetGrowthForAccount();
                UpdateChart(1);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateChart(int eventType = 0)
        {

            var selectedGrowthChartType = DominatorAccountViewModel.GrowthChartType;

            if (selectedGrowthChartType == "Total")
            {

                DominatorAccountViewModel.SeriesCollection.FirstOrDefault().Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Total");
                if (DominatorAccountViewModel.SeriesCollection.Count > 1)
                    DominatorAccountViewModel.SeriesCollection.RemoveAt(1);
                DominatorAccountViewModel.Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
                //DominatorAccountViewModel.Labels = GetChartLabels();
                DominatorAccountViewModel.YFormatter = value => value.ToString();

            }
            else if (selectedGrowthChartType == "Gain")
            {
                DominatorAccountViewModel.SeriesCollection.FirstOrDefault().Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Gain");
                if (DominatorAccountViewModel.SeriesCollection.Count > 1)
                    DominatorAccountViewModel.SeriesCollection.RemoveAt(1);
                DominatorAccountViewModel.Labels = GetChartLabels();
                DominatorAccountViewModel.YFormatter = value => value.ToString();
            }

            else
            {
                if (DominatorAccountViewModel.SeriesCollection.Count() < 2 && eventType != 1)
                {

                    DominatorAccountViewModel.SeriesCollection.FirstOrDefault().Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Total");
                    var series = new LineSeries
                    {
                        Title = DominatorAccountViewModel.GrowthChartProperty + " Gain",
                        Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Gain")
                    };
                    DominatorAccountViewModel.SeriesCollection.Add(series);
                }
                else if (DominatorAccountViewModel.SeriesCollection.Count() == 2 && eventType == 1)
                {
                    DominatorAccountViewModel.SeriesCollection.FirstOrDefault().Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Total");
                    DominatorAccountViewModel.SeriesCollection.ElementAt(1).Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Gain");
                }

                DominatorAccountViewModel.Labels = GetChartLabels();
                DominatorAccountViewModel.YFormatter = value => value.ToString();
            }



        }
        private void PlotChart()
        {
            this.Dispatcher.Invoke(() =>
            {

                DominatorAccountViewModel.GrowthList = new List<DominatorHouseCore.ViewModel.DailyStatisticsViewModel>();

                // DominatorAccountViewModel.GrowthList = accountUpdateFactory.GetDailyGrowthForAccount(DominatorAccountViewModel.GrowthChartAccountNumber, GetValueFromDescription<GrowthChartPeriod>(DominatorAccountViewModel.GrowthChartPeriod));
                //DominatorAccountViewModel.GrowthList = accountUpdateFactory.GetDailyGrowthForAccount(DominatorAccountViewModel.GrowthChartAccountNumber, GrowthChartPeriod.Past30Days);

                GetGrowthForAccount();
                if (DominatorAccountViewModel.GrowthChartType == "Total" || DominatorAccountViewModel.GrowthChartType == "Both")
                {
                    DominatorAccountViewModel.SeriesCollection.FirstOrDefault().Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Total");



                }
                if (DominatorAccountViewModel.GrowthChartType == "Gain" || DominatorAccountViewModel.GrowthChartType == "Both")
                {


                    DominatorAccountViewModel.SeriesCollection.FirstOrDefault().Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, DominatorAccountViewModel.GrowthChartProperty, "Gain");


                }

                DominatorAccountViewModel.Labels = GetChartLabels();
                DominatorAccountViewModel.YFormatter = value => value.ToString();
            });

        }

        private void GetGrowthForAccount()
        {
            var accountUpdateFactory = SocinatorInitialize
                 .GetSocialLibrary(SocialNetworks.Twitter)
                 .GetNetworkCoreFactory().AccountUpdateFactory;
            DominatorAccountViewModel.GrowthList = accountUpdateFactory.GetDailyGrowthForAccount(DominatorAccountViewModel.GrowthChartAccountNumber, GetValueFromDescription<GrowthChartPeriod>(DominatorAccountViewModel.GrowthChartPeriod));
        }

        private string[] GetChartLabels()
        {

            if (DominatorAccountViewModel.GrowthChartPeriod == "Past 30 days" || DominatorAccountViewModel.GrowthChartPeriod == "Past week")
            {

                string[] datesArray = DominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();

                return datesArray;

            }
            if (DominatorAccountViewModel.GrowthChartPeriod == "Past day")
            {

                string[] datesArray = DominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();

                return datesArray;

            }

            return new string[] { };
        }

        private ChartValues<int> getGrowthValueList(List<DailyStatisticsViewModel> growthList, string growthChartProperty, string type)
        {
            var list = new ChartValues<int>();
            if (growthChartProperty == "Followers")
            {

                foreach (var g in growthList)
                {
                    list.Add(g.GrowthColumnValue1);
                }
            }
            if (growthChartProperty == "Followings")
            {
                foreach (var g in growthList)
                {
                    list.Add(g.GrowthColumnValue2);
                }
            }
            if (growthChartProperty == "Tweets")
            {
                foreach (var g in growthList)
                {
                    list.Add(g.GrowthColumnValue3);
                }
            }


            if (type == "Gain")
            {
                var gainList = new ChartValues<int>();
                for (int i = 0; i < list.Count; i++)
                {

                    var value = i == 0 ? 0 : (list[i] - list[i - 1]);
                    //var value = i == 0 ? 0 : (i == 1 ? list[i] - 0 : (list[i] - list[i - 1]));
                    gainList.Add(value);
                }
                list = gainList;
            }

            return list;
        }

        private void CmbboxGrowthChartPeriod_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                GetGrowthForAccount();
                UpdateChart(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void CmbboxGrowthChartType_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                UpdateChart(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void CmbboxGrowthType_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                UpdateChart(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }


    }
}
