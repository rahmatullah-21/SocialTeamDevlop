using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Factories;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
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
        private readonly DominatorAccountViewModel _dominatorAccountViewModel;
        private readonly ISelectedNetworkViewModel _selectedNetworkViewModel;
        private readonly BackgroundWorker worker = new BackgroundWorker();



        private AccountGrowthControl()
        {

            _dominatorAccountViewModel = (DominatorAccountViewModel)DominatorHouseCore.IoC.Container.Resolve<IDominatorAccountViewModel>();
            _selectedNetworkViewModel = DominatorHouseCore.IoC.Container.Resolve<ISelectedNetworkViewModel>();
            InitializeComponent();
            _dominatorAccountViewModel.AccountCollectionView =
                CollectionViewSource.GetDefaultView(_dominatorAccountViewModel.LstDominatorAccountModel);
            AccountModule.DataContext = _dominatorAccountViewModel;
            InitializeChart();
            worker.DoWork += ReloadGridWithGrowth;
            worker.RunWorkerAsync();

        }

        private void InitializeChart()
        {

            try
            {
                _dominatorAccountViewModel.GrowthChartAccountNetwork = _dominatorAccountViewModel.LstDominatorAccountModel[0].AccountBaseModel.AccountNetwork;
                _dominatorAccountViewModel.GrowthProperties = _dominatorAccountViewModel.LstDominatorAccountModel[0].AccountBaseModel.GetGrowthProperties(_dominatorAccountViewModel.GrowthChartAccountNetwork);
                _dominatorAccountViewModel.GrowthChartPeriods = GetChartPeriodEnumStringList();
                _dominatorAccountViewModel.GrowthChartTypes = new List<string>() { "Gain", "Total", "Both" };
                _dominatorAccountViewModel.GrowthChartAccountNumber = _dominatorAccountViewModel.LstDominatorAccountModel[0].AccountId;

                _dominatorAccountViewModel.GrowthChartPeriod = "Past 30 days";
                _dominatorAccountViewModel.GrowthChartProperty = string.Join(",", _dominatorAccountViewModel.GrowthProperties.Select(x => x.PropertyName));
                _dominatorAccountViewModel.GrowthChartType = "Total";
                _dominatorAccountViewModel.SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = _dominatorAccountViewModel.GrowthProperties[0].PropertyName.ToString(),
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
            _accountGrowthInstance.GetRespectiveAccounts(GrowthPeriod.Daily);
            PlotChart();
        }
        private void UpdateChart(int eventType = 0)
        {
            try
            {
                var count = 0;
                var selectedGrowthChartType = _dominatorAccountViewModel.GrowthChartType;
                List<string> chartProperties = _dominatorAccountViewModel.GrowthChartProperty.Split(',').ToList();
                if (_dominatorAccountViewModel.SeriesCollection?.Count > 1)
                {
                    try
                    {
                        var iterations = _dominatorAccountViewModel.SeriesCollection.Count;
                        for (var i = 0; i < iterations; i++)
                        {
                            if (_dominatorAccountViewModel.SeriesCollection.Count > 1)
                                _dominatorAccountViewModel.SeriesCollection.RemoveAt(1);
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
                                Values = getGrowthValueList(_dominatorAccountViewModel.GrowthList, property, "Total")
                            };
                            if (count == 0)
                            {
                                _dominatorAccountViewModel.SeriesCollection[0] = series;
                            }
                            else
                            {
                                _dominatorAccountViewModel.SeriesCollection.Add(series);
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
                                Values = getGrowthValueList(_dominatorAccountViewModel.GrowthList, property, "Gain")
                            };

                            if (count == 0)
                            {
                                _dominatorAccountViewModel.SeriesCollection[0] = series;
                            }
                            else
                            {
                                _dominatorAccountViewModel.SeriesCollection.Add(series);
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
                                Values = getGrowthValueList(_dominatorAccountViewModel.GrowthList, property, "Total")
                            };
                            if (count == 0)
                            {
                                _dominatorAccountViewModel.SeriesCollection[0] = series;
                            }
                            else
                            {
                                _dominatorAccountViewModel.SeriesCollection.Add(series);
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
                                Values = getGrowthValueList(_dominatorAccountViewModel.GrowthList, property, "Gain")
                            };


                            _dominatorAccountViewModel.SeriesCollection.Add(series);

                            count++;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }
                // _dominatorAccountViewModel.Labels = GetChartLabels();
                _dominatorAccountViewModel.YFormatter = value => value.ToString();
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

                _dominatorAccountViewModel.GrowthList = new List<DailyStatisticsViewModel>();
                GetGrowthForAccount();
                UpdateChart(1);

                _dominatorAccountViewModel.Labels = GetChartLabels();
                _dominatorAccountViewModel.YFormatter = value => value.ToString();
            });

        }
        private void GetGrowthForAccount()
        {
            var accountUpdateFactory = SocinatorInitialize
                 .GetSocialLibrary(_dominatorAccountViewModel.GrowthChartAccountNetwork)
                 .GetNetworkCoreFactory().AccountUpdateFactory;
            _dominatorAccountViewModel.GrowthList = accountUpdateFactory.GetDailyGrowthForAccount(_dominatorAccountViewModel.GrowthChartAccountNumber, GetValueFromDescription<GrowthChartPeriod>(_dominatorAccountViewModel.GrowthChartPeriod));
        }

        private string[] GetChartLabels()
        {

            if (_dominatorAccountViewModel.GrowthChartPeriod == "Past 30 days" || _dominatorAccountViewModel.GrowthChartPeriod == "Past week")
            {

                string[] datesArray = _dominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();
                chart.AxisX[0].Title = "Days";
                return datesArray;

            }
            if (_dominatorAccountViewModel.GrowthChartPeriod == "Past 3 Months" || _dominatorAccountViewModel.GrowthChartPeriod == "Past 6 Months")
            {

                string[] datesArray = _dominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MMMM")).ToArray();
                chart.AxisX[0].Title = "Months";
                return datesArray;

            }
            if (_dominatorAccountViewModel.GrowthChartPeriod == "Past day")
            {

                string[] datesArray = _dominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();
                chart.AxisX[0].Title = "Days";
                return datesArray;

            }
            if (_dominatorAccountViewModel.GrowthChartPeriod == "All time")
            {

                try
                {
                    if (_dominatorAccountViewModel.GrowthList.LastOrDefault().Date <=
                                _dominatorAccountViewModel.GrowthList.FirstOrDefault().Date.AddDays(-30))
                    {
                        string[] datesArray = _dominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MMMM")).ToArray();
                        chart.AxisX[0].Title = "Months";
                        return datesArray;

                    }
                    else
                    {
                        string[] datesArray = _dominatorAccountViewModel.GrowthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();
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
            var properties = _dominatorAccountViewModel.LstDominatorAccountModel.Where(x => x.AccountId == _dominatorAccountViewModel.GrowthChartAccountNumber).FirstOrDefault().AccountBaseModel.GrowthProperties;
            properties = _dominatorAccountViewModel.LstDominatorAccountModel.Where(x => x.AccountId == _dominatorAccountViewModel.GrowthChartAccountNumber).FirstOrDefault().AccountBaseModel.
                GetGrowthProperties(_dominatorAccountViewModel.LstDominatorAccountModel.Where(x => x.AccountId == _dominatorAccountViewModel.GrowthChartAccountNumber).FirstOrDefault().AccountBaseModel.AccountNetwork);
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

        private static AccountGrowthControl _accountGrowthInstance = null;

        public static AccountGrowthControl GetAccountGrowthControl(SocialNetworks socialNetwork, AccessorStrategies strategies)
        {
            if (_accountGrowthInstance == null)
            {
                _accountGrowthInstance = new AccountGrowthControl();
            }

            _accountGrowthInstance.GetRespectiveAccounts();

            return _accountGrowthInstance;
        }


        public static AccountGrowthControl GetAccountGrowthControl(AccessorStrategies strategies)
        {
            return _accountGrowthInstance ?? (_accountGrowthInstance = new AccountGrowthControl());
        }
        public static AccountGrowthControl GetAccountGrowthControl(SocialNetworks socialNework)
        {
            return _accountGrowthInstance;
        }

        public void GetRespectiveAccounts(GrowthPeriod period = GrowthPeriod.NoPeriod)
        {


            try
            {
                var listCollection = (ListCollectionView)_dominatorAccountViewModel.AccountCollectionView;
                _dominatorAccountViewModel.GrowthProperties = _dominatorAccountViewModel.LstDominatorAccountModel[0].AccountBaseModel.GrowthProperties;
                if (period == GrowthPeriod.NoPeriod)
                {
                    _dominatorAccountViewModel.LstDominatorAccountModel.ForEach(x =>
                    {
                        x.IsAccountManagerAccountSelected = false;
                        x.DisplayColumnValue6 = 0;
                        x.DisplayColumnValue7 = 0;
                        x.DisplayColumnValue8 = 0;
                        x.DisplayColumnValue9 = 0;
                        x.DisplayColumnValue10 = 0;
                    });

                    listCollection.Filter = x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == _selectedNetworkViewModel.Selected;
                }
                else
                {
                    _dominatorAccountViewModel.LstDominatorAccountModel.ForEach(x =>
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
                    });

                    this.Dispatcher.Invoke(() =>
                    {
                        listCollection.Filter = x => ((DominatorAccountModel)x).AccountBaseModel.AccountNetwork == _selectedNetworkViewModel.Selected;

                    });
                }

                if (_selectedNetworkViewModel.Selected == SocialNetworks.Social)
                    this.Dispatcher.Invoke(() =>
                    {
                        listCollection.Filter = null;

                    });


                var spec = (!_selectedNetworkViewModel.Selected.HasValue || _selectedNetworkViewModel.Selected == SocialNetworks.Social) ?
                   DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider() :
                   SocinatorInitialize.GetSocialLibrary(_selectedNetworkViewModel.Selected.Value)
                         .GetNetworkCoreFactory()
                         .AccountCountFactory.GetColumnSpecificationProvider();


                _dominatorAccountViewModel.GrowthChartAccountNumber = (listCollection.GetItemAt(0) as DominatorAccountModel).AccountId;
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
        private void HandleGrowthTypeCheck(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (_dominatorAccountViewModel.GrowthChartProperty == null)
                _dominatorAccountViewModel.GrowthChartProperty = "";
            List<string> chartProperties = _dominatorAccountViewModel.GrowthChartProperty.Split(',').ToList();

            if (!chartProperties.Contains(cb.Content.ToString()))
                chartProperties.Add(cb.Content.ToString());

            _dominatorAccountViewModel.GrowthChartProperty = string.Join(",", chartProperties.ToArray());
            if (_dominatorAccountViewModel.SeriesCollection != null)
                UpdateChart(1);
        }
        private void HandleGrowthTypeUnCheck(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cb = sender as CheckBox;
                List<string> chartProperties = _dominatorAccountViewModel.GrowthChartProperty.Split(',').ToList();

                var index = chartProperties.IndexOf(cb.Content.ToString());
                chartProperties.RemoveAt(index);

                _dominatorAccountViewModel.GrowthChartProperty = string.Join(",", chartProperties.ToArray());
                if (_dominatorAccountViewModel.SeriesCollection != null)
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

                _accountGrowthInstance.GetRespectiveAccounts(selectedGrowthPeriod);

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
                _dominatorAccountViewModel.GrowthChartAccountNetwork = (cmbGrowthAcount.SelectedItem as DominatorAccountModel).AccountBaseModel.AccountNetwork;
                var property = (cmbGrowthAcount.SelectedItem as DominatorAccountModel).AccountBaseModel.GetGrowthProperties(_dominatorAccountViewModel.GrowthChartAccountNetwork);
                _dominatorAccountViewModel.GrowthChartProperty = string.Join(",", (property.Select(x => x.PropertyName)));
                _dominatorAccountViewModel.GrowthProperties = property;
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

                _accountGrowthInstance.GetRespectiveAccounts(selectedGrowthPeriod);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
