using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Factories;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Unity;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountGrowthControl.xaml
    /// </summary>
    public partial class AccountGrowthControl : UserControl, INotifyPropertyChanged
    {
        private DominatorAccountViewModel _dominatorAccountViewModel;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private List<GrowthProperty> GrowthProperties = new List<GrowthProperty>();
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



        private AccountGrowthControl(AccessorStrategies strategyPack)
        {

            _dominatorAccountViewModel = (DominatorAccountViewModel)DominatorHouseCore.IoC.Container.Resolve<IDominatorAccountViewModel>();
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

            try
            {
                DominatorAccountViewModel.GrowthChartAccountNetwork = DominatorAccountViewModel.LstDominatorAccountModel[0].AccountBaseModel.AccountNetwork;
                DominatorAccountViewModel.GrowthProperties = DominatorAccountViewModel.LstDominatorAccountModel[0].AccountBaseModel.GetGrowthProperties(DominatorAccountViewModel.GrowthChartAccountNetwork);
                DominatorAccountViewModel.GrowthChartPeriods = GetChartPeriodEnumStringList();
                DominatorAccountViewModel.GrowthChartTypes = new List<string>() { "Gain", "Total", "Both" };
                DominatorAccountViewModel.GrowthChartAccountNumber = DominatorAccountViewModel.LstDominatorAccountModel[0].AccountId;

                DominatorAccountViewModel.GrowthChartPeriod = "Past 30 days";
                DominatorAccountViewModel.GrowthChartProperty = string.Join(",", DominatorAccountViewModel.GrowthProperties.Select(x => x.PropertyName));
                DominatorAccountViewModel.GrowthChartType = "Total";
                DominatorAccountViewModel.SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = DominatorAccountViewModel.GrowthProperties[0].PropertyName.ToString(),
                    Values = new ChartValues<double> {0,1,3,4,5, 6,7,8,11,10,20,19,19,19,19,19,19,19,20,20,21,21,24,24,24,24,24,28,29,29 }
                }

            };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

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


        private void ReloadGridWithGrowth(object sender, DoWorkEventArgs e)
        {
            _accountGrowthInstance.GetRespectiveAccounts(socialNetworks, GrowthPeriod.Daily);
            PlotChart();
        }
        private void UpdateChart(int eventType = 0)
        {
            try
            {
                var count = 0;
                var selectedGrowthChartType = DominatorAccountViewModel.GrowthChartType;
                List<string> chartProperties = DominatorAccountViewModel.GrowthChartProperty.Split(',').ToList();
                if (DominatorAccountViewModel.SeriesCollection?.Count > 1)
                {
                    try
                    {
                        var iterations = DominatorAccountViewModel.SeriesCollection.Count;
                        for (var i = 0; i < iterations; i++)
                        {
                            if (DominatorAccountViewModel.SeriesCollection.Count > 1)
                                DominatorAccountViewModel.SeriesCollection.RemoveAt(1);
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                if (selectedGrowthChartType == "Total")
                {
                    foreach (var property in chartProperties)
                    {
                        try
                        {
                            var series = new LineSeries
                            {
                                Title = property + " Total",
                                Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, property, "Total")
                            };
                            if (count == 0)
                            {
                                DominatorAccountViewModel.SeriesCollection[0] = series;
                            }
                            else
                            {
                                DominatorAccountViewModel.SeriesCollection.Add(series);
                            }
                            count++;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }




                }
                else if (selectedGrowthChartType == "Gain")
                {

                    foreach (var property in chartProperties)
                    {
                        try
                        {
                            var series = new LineSeries
                            {
                                Title = property + " Gain",
                                Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, property, "Gain")
                            };

                            if (count == 0)
                            {
                                DominatorAccountViewModel.SeriesCollection[0] = series;
                            }
                            else
                            {
                                DominatorAccountViewModel.SeriesCollection.Add(series);
                            }
                            count++;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                }

                else
                {
                    foreach (var property in chartProperties)
                    {
                        try
                        {
                            var series = new LineSeries
                            {
                                Title = property + " Total",
                                Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, property, "Total")
                            };
                            if (count == 0)
                            {
                                DominatorAccountViewModel.SeriesCollection[0] = series;
                            }
                            else
                            {
                                DominatorAccountViewModel.SeriesCollection.Add(series);
                            }
                            count++;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    foreach (var property in chartProperties)
                    {
                        try
                        {
                            var series = new LineSeries
                            {
                                Title = property + " Gain",
                                Values = getGrowthValueList(DominatorAccountViewModel.GrowthList, property, "Gain")
                            };


                            DominatorAccountViewModel.SeriesCollection.Add(series);

                            count++;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }
                // DominatorAccountViewModel.Labels = GetChartLabels();
                DominatorAccountViewModel.YFormatter = value => value.ToString();
                chart.AxisX[0].Labels = GetChartLabels();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }



        }
        private void PlotChart()
        {
            this.Dispatcher.Invoke(() =>
            {

                DominatorAccountViewModel.GrowthList = new List<DominatorHouseCore.ViewModel.DailyStatisticsViewModel>();
                GetGrowthForAccount();
                UpdateChart(1);

                DominatorAccountViewModel.Labels = GetChartLabels();
                DominatorAccountViewModel.YFormatter = value => value.ToString();
            });

        }
        private void GetGrowthForAccount()
        {
            var accountUpdateFactory = SocinatorInitialize
                 .GetSocialLibrary(DominatorAccountViewModel.GrowthChartAccountNetwork)
                 .GetNetworkCoreFactory().AccountUpdateFactory;
            DominatorAccountViewModel.GrowthList = accountUpdateFactory.GetDailyGrowthForAccount(DominatorAccountViewModel.GrowthChartAccountNumber, GetValueFromDescription<GrowthChartPeriod>(DominatorAccountViewModel.GrowthChartPeriod));
        }

        private string[] GetChartLabels()
        {

            if (DominatorAccountViewModel.GrowthChartPeriod == "Past 30 days" || DominatorAccountViewModel.GrowthChartPeriod == "Past week")
            {

                string[] datesArray = DominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();
                chart.AxisX[0].Title = "Days";
                return datesArray;

            }
            if (DominatorAccountViewModel.GrowthChartPeriod == "Past 3 Months" || DominatorAccountViewModel.GrowthChartPeriod == "Past 6 Months")
            {

                string[] datesArray = DominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MMMM")).ToArray();
                chart.AxisX[0].Title = "Months";
                return datesArray;

            }
            if (DominatorAccountViewModel.GrowthChartPeriod == "Past day")
            {

                string[] datesArray = DominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();
                chart.AxisX[0].Title = "Days";
                return datesArray;

            }
            if (DominatorAccountViewModel.GrowthChartPeriod == "All time")
            {

                try
                {
                    if (DominatorAccountViewModel.GrowthList.LastOrDefault().Date <=
                                DominatorAccountViewModel.GrowthList.FirstOrDefault().Date.AddDays(-30))
                    {
                        string[] datesArray = DominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MMMM")).ToArray();
                        chart.AxisX[0].Title = "Months";
                        return datesArray;

                    }
                    else
                    {
                        string[] datesArray = DominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();
                        chart.AxisX[0].Title = "Days";
                        return datesArray;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }

            return new string[] { };
        }
        private ChartValues<int> getGrowthValueList(List<DailyStatisticsViewModel> growthList, string growthChartProperty, string type)
        {
            var list = new ChartValues<int>();
            var properties = DominatorAccountViewModel.LstDominatorAccountModel.Where(x => x.AccountId == DominatorAccountViewModel.GrowthChartAccountNumber).FirstOrDefault().AccountBaseModel.GrowthProperties;
            properties = DominatorAccountViewModel.LstDominatorAccountModel.Where(x => x.AccountId == DominatorAccountViewModel.GrowthChartAccountNumber).FirstOrDefault().AccountBaseModel.
                GetGrowthProperties(DominatorAccountViewModel.LstDominatorAccountModel.Where(x => x.AccountId == DominatorAccountViewModel.GrowthChartAccountNumber).FirstOrDefault().AccountBaseModel.AccountNetwork);
            for (int i = 1; i <= properties.Count(); i++)
            {
                if (growthChartProperty == properties[i - 1].PropertyName)
                {
                    foreach (var g in growthList)
                    {
                        var propertyName = "GrowthColumnValue" + i;
                        System.Reflection.PropertyInfo prop = typeof(DailyStatisticsViewModel).GetProperty(propertyName);
                        object value = prop.GetValue(g);

                        list.Add(Convert.ToInt32(value));
                    }
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

        public static AccountGrowthControl GetAccountGrowthControl(SocialNetworks socialNetwork, AccessorStrategies strategies)
        {
            socialNetworks = socialNetwork;
            if (_accountGrowthInstance == null)
            {
                _accountGrowthInstance = new AccountGrowthControl(strategies);
            }

            _accountGrowthInstance.GetRespectiveAccounts(socialNetworks);

            return _accountGrowthInstance;
        }


        public static AccountGrowthControl GetAccountGrowthControl(AccessorStrategies strategies)
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


            try
            {
                var listCollection = (ListCollectionView)DominatorAccountViewModel.AccountCollectionView;
                DominatorAccountViewModel.GrowthProperties = DominatorAccountViewModel.LstDominatorAccountModel[0].AccountBaseModel.GrowthProperties;
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
                        //for (int i = 1; i <= x.AccountBaseModel.GrowthProperties.Count(); i++)
                        //{
                        //    var propertyName = "GrowthColumnValue" + i;
                        //    System.Reflection.PropertyInfo prop = typeof(DailyStatisticsViewModel).GetProperty(propertyName);
                        //    object value = prop.GetValue(AccoutGrowth);
                        //    x.AccountBaseModel.GrowthProperties[i - 1].PropertyValue = Convert.ToInt32(value);
                        //}

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
                    this.Dispatcher.Invoke(() =>
                    {
                        listCollection.Filter = null;

                    });


                var spec = (socialNetworks == SocialNetworks.Social) ?
                   DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider() :
                   SocinatorInitialize.GetSocialLibrary(socialNetworks)
                         .GetNetworkCoreFactory()
                         .AccountCountFactory.GetColumnSpecificationProvider();


                DominatorAccountViewModel.GrowthChartAccountNumber = (listCollection.GetItemAt(0) as DominatorAccountModel).AccountId;
                DominatorAccountViewModel.SocialNetwork = socialNetworks;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

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
        //         exception.DebugLog();
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
        private void HandleGrowthTypeCheck(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (DominatorAccountViewModel.GrowthChartProperty == null)
                DominatorAccountViewModel.GrowthChartProperty = "";
            List<string> chartProperties = DominatorAccountViewModel.GrowthChartProperty.Split(',').ToList();

            if (!chartProperties.Contains(cb.Content.ToString()))
                chartProperties.Add(cb.Content.ToString());

            DominatorAccountViewModel.GrowthChartProperty = string.Join(",", chartProperties.ToArray());
            if (DominatorAccountViewModel.SeriesCollection != null)
                UpdateChart(1);
        }
        private void HandleGrowthTypeUnCheck(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cb = sender as CheckBox;
                List<string> chartProperties = DominatorAccountViewModel.GrowthChartProperty.Split(',').ToList();

                var index = chartProperties.IndexOf(cb.Content.ToString());
                chartProperties.RemoveAt(index);

                DominatorAccountViewModel.GrowthChartProperty = string.Join(",", chartProperties.ToArray());
                if (DominatorAccountViewModel.SeriesCollection != null)
                    UpdateChart(1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
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

        private void CmbboxGrowthChartAccount_OnDropDownClosed(object sender, EventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                var currentItem = cb.DataContext as DominatorAccountViewModel;
                DominatorAccountViewModel.GrowthChartAccountNetwork = (cmbGrowthAcount.SelectedItem as DominatorAccountModel).AccountBaseModel.AccountNetwork;
                DominatorAccountViewModel.GrowthChartProperty = string.Join(",", (cmbGrowthAcount.SelectedItem as DominatorAccountModel).AccountBaseModel.GrowthProperties.Select(x => x.PropertyName));
                DominatorAccountViewModel.GrowthProperties = (cmbGrowthAcount.SelectedItem as DominatorAccountModel).AccountBaseModel.GrowthProperties;
                //(currentItem.AccountCollectionView.CurrentItem as DominatorAccountModel).AccountBaseModel.AccountNetwork;
                GetGrowthForAccount();
                UpdateChart(1);


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
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void cmbGrowthChartPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void cmbGrowthAcount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                var currentItem = cb.DataContext as DominatorAccountViewModel;
                DominatorAccountViewModel.GrowthChartAccountNetwork = (cmbGrowthAcount.SelectedItem as DominatorAccountModel).AccountBaseModel.AccountNetwork;
                var property = (cmbGrowthAcount.SelectedItem as DominatorAccountModel).AccountBaseModel.GetGrowthProperties(DominatorAccountViewModel.GrowthChartAccountNetwork);
                DominatorAccountViewModel.GrowthChartProperty = string.Join(",", (property.Select(x => x.PropertyName)));
                DominatorAccountViewModel.GrowthProperties = property;
                //(currentItem.AccountCollectionView.CurrentItem as DominatorAccountModel).AccountBaseModel.AccountNetwork;
                GetGrowthForAccount();
                UpdateChart(1);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void cmbGrowthChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void cmbGrowthPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
    }
}
