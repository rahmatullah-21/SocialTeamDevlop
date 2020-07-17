using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorHouseCore.ViewModel.Common;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace DominatorUIUtility.ViewModel
{
    public interface IAccountGrowthControlViewModel
    {

    }

    public class AccountGrowthControlViewModel : Prism.Mvvm.BindableBase, IAccountGrowthControlViewModel
    {
        private readonly object _syncObject = new object();
        public IAccountCollectionViewModel Accounts { get; }
        public ISelectedNetworkViewModel SelectedNetworkViewModel { get; }
        private readonly IAccountGrowthPropertiesProvider _accountGrowthPropertiesProvider;
        private SeriesCollection _seriesCollection;
        private DominatorAccountModel _selectedAccount;
        private string _axisXTitle;

        public ObservableCollection<DailyStatisticsViewModel> GrowthList { get; set; }

        public string AxisXTitle
        {
            get { return _axisXTitle; }
            set { SetProperty(ref _axisXTitle, value); }
        }

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { SetProperty(ref _seriesCollection, value); }
        }

        public DominatorAccountModel SelectedAccount
        {
            get { return _selectedAccount; }
            set { SetProperty(ref _selectedAccount, value); }
        }

        public ObservableCollection<GrowthProperty> GrowthProperties { get; }


        public ObservableCollection<string> AxisXLabels { get; }

        public SelectableViewModel<GrowthChartPeriod?> GrowthChartPeriods { get; }
        public SelectableViewModel<GrowthChartType?> GrowthChartTypes { get; }
        public SelectableViewModel<GrowthPeriod?> GrowthPeriods { get; }
        public string GrowthChartProperty => string.Join(",", GrowthProperties.Where(a => a.IsChecked).Select(x => x.PropertyName));
        public DelegateCommand AccountSelectionChangedCmd { get; }
        public DelegateCommand<GrowthProperty> GrowthPropertyCheckedCmd { get; }


        public AccountGrowthControlViewModel(IAccountCollectionViewModel accountCollectionViewModel, IAccountGrowthPropertiesProvider accountGrowthPropertiesProvider, ISelectedNetworkViewModel selectedNetworkViewModel)
        {
            Accounts = accountCollectionViewModel;
            SelectedNetworkViewModel = selectedNetworkViewModel;
            SelectedNetworkViewModel.ItemSelected += NetworkItemSelected;
            _accountGrowthPropertiesProvider = accountGrowthPropertiesProvider;
            AccountSelectionChangedCmd = new DelegateCommand(OnAccountSelectionChanged);
            GrowthPropertyCheckedCmd = new DelegateCommand<GrowthProperty>(OnGrowthPeropertyChecked);

            SelectedAccount = Accounts.FirstOrDefault();
            GrowthProperties = new ObservableCollection<GrowthProperty>(_accountGrowthPropertiesProvider[SelectedAccount?.AccountBaseModel?.AccountNetwork ?? SocialNetworks.Admin]);
            BindingOperations.EnableCollectionSynchronization(GrowthProperties, _syncObject);

            AxisXLabels = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(AxisXLabels, _syncObject);

            GrowthList = new ObservableCollection<DailyStatisticsViewModel>();
            BindingOperations.EnableCollectionSynchronization(GrowthList, _syncObject);

            GrowthPeriods = new SelectableViewModel<GrowthPeriod?>(Enum.GetValues(typeof(GrowthPeriod)).Cast<GrowthPeriod?>(), GrowthPeriod.Daily);
            GrowthPeriods.ItemSelected += GrowthPeriodsOnItemSelectionChanged;

            GrowthChartPeriods = new SelectableViewModel<GrowthChartPeriod?>(Enum.GetValues(typeof(GrowthChartPeriod)).Cast<GrowthChartPeriod?>(), GrowthChartPeriod.Past30Days);
            GrowthChartPeriods.ItemSelected += GrowthChartPeriodsSelectionChanged;

            GrowthChartTypes = new SelectableViewModel<GrowthChartType?>(Enum.GetValues(typeof(GrowthChartType)).Cast<GrowthChartType?>(), GrowthChartType.Total);
            GrowthChartTypes.ItemSelected += GrowthChartTypesSelectionChanged;



            SeriesCollection = new SeriesCollection();
            SetRespectiveAccounts(GrowthPeriods.Selected ?? GrowthPeriod.Daily);
            SetGrowthForAccount();
            UpdateChart();
        }

        private void NetworkItemSelected(object sender, SocialNetworks? e)
        {
            lock (_syncObject)
            {
                SelectedAccount = Accounts.BySocialNetwork(e ?? SocialNetworks.Admin).FirstOrDefault();
                UpdateGrowthProperties();
                SetGrowthForAccount();
                //UpdateChart();
            }
        }

        private void OnGrowthPeropertyChecked(GrowthProperty growthProperty)
        {
            lock (_syncObject)
            {
                UpdateChart();
            }
        }

        private void OnAccountSelectionChanged()
        {
            lock (_syncObject)
            {
                UpdateGrowthProperties();
                SetGrowthForAccount();
                UpdateChart();
            }
        }

        private void GrowthPeriodsOnItemSelectionChanged(object sender, GrowthPeriod? e)
        {
            lock (_syncObject)
            {
                SetRespectiveAccounts(e ?? GrowthPeriod.NoPeriod);
            }
        }

        private void GrowthChartPeriodsSelectionChanged(object sender, GrowthChartPeriod? e)
        {
            lock (_syncObject)
            {
                SetGrowthForAccount();
                UpdateChart();
            }
        }

        private void GrowthChartTypesSelectionChanged(object sender, GrowthChartType? e)
        {
            lock (_syncObject)
            {
                UpdateChart();
            }
        }


        private void UpdateGrowthProperties()
        {
            GrowthProperties.Clear();
            GrowthProperties.AddRange(
                _accountGrowthPropertiesProvider[SelectedAccount?.AccountBaseModel?.AccountNetwork ?? SocialNetworks.Admin]);
        }

        private void SetRespectiveAccounts(GrowthPeriod period)
        {
            try
            {
                lock (_syncObject)
                {
                    UpdateGrowthProperties();
                    if (period == GrowthPeriod.NoPeriod)
                    {
                        Accounts.GetCopySync().ForEach(x =>
                        {
                            x.IsAccountManagerAccountSelected = false;
                            x.DisplayColumnValue6 = 0;
                            x.DisplayColumnValue7 = 0;
                            x.DisplayColumnValue8 = 0;
                            x.DisplayColumnValue9 = 0;
                            x.DisplayColumnValue10 = 0;
                        });
                    }
                    else
                    {
                        Accounts.GetCopySync().ForEach(x =>
                        {
                            var accountUpdateFactory = SocinatorInitialize
                                .GetSocialLibrary(x.AccountBaseModel.AccountNetwork)
                                .GetNetworkCoreFactory().AccountUpdateFactory;
                            x.IsAccountManagerAccountSelected = false;

                            var accoutGrowth = accountUpdateFactory.GetDailyGrowth(x.AccountId, x.AccountBaseModel.ProfileId, period);

                            x.DisplayColumnValue6 = accoutGrowth?.GrowthColumnValue1 ?? 0;
                            x.DisplayColumnValue7 = accoutGrowth?.GrowthColumnValue2 ?? 0;
                            x.DisplayColumnValue8 = accoutGrowth?.GrowthColumnValue3 ?? 0;
                            x.DisplayColumnValue9 = accoutGrowth?.GrowthColumnValue4 ?? 0;
                            x.DisplayColumnValue10 = accoutGrowth?.GrowthColumnValue5 ?? 0;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void UpdateSeriesCollectionFor(List<string> chartProperties, GrowthChartType type)
        {
            foreach (var property in chartProperties)
            {
                SeriesCollection.Add(new LineSeries
                {
                    Title = $"{property} {type.GetDescriptionAttr().FromResourceDictionary()}",
                    Values = GetGrowthValueList(GrowthList, property, type)
                });
            }
        }


        private void UpdateChart()
        {
            try
            {
                var selectedGrowthChartType = GrowthChartTypes.Selected ?? GrowthChartType.Total;
                List<string> chartProperties = GrowthProperties.Where(a => a.IsChecked).Select(x => x.PropertyName).ToList();
                SeriesCollection.Clear();
                switch (selectedGrowthChartType)
                {
                    case GrowthChartType.Gain:
                    case GrowthChartType.Total:
                        UpdateSeriesCollectionFor(chartProperties, selectedGrowthChartType);
                        break;
                    case GrowthChartType.Both:
                        UpdateSeriesCollectionFor(chartProperties, GrowthChartType.Total);
                        UpdateSeriesCollectionFor(chartProperties, GrowthChartType.Gain);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private ChartValues<int> GetGrowthValueList(ObservableCollection<DailyStatisticsViewModel> growthList, string growthChartProperty, GrowthChartType type)
        {
            var list = new ChartValues<int>();
            var properties = _accountGrowthPropertiesProvider
                [SelectedAccount?.AccountBaseModel?.AccountNetwork ?? SocialNetworks.Admin];
            for (int i = 1; i <= properties.Count(); i++)
            {
                if (growthChartProperty == properties[i - 1].PropertyName)
                {
                    list.AddRange(growthList.Select(a => a[i]));
                }
            }


            if (type == GrowthChartType.Gain)
            {
                var gainList = new ChartValues<int>();
                for (int i = 0; i < list.Count; i++)
                {

                    var value = i == 0 ? 0 : (list[i] - list[i - 1]);
                    gainList.Add(value);
                }
                list = gainList;
            }

            return list;
        }

        private void SetGrowthForAccount()
        {
            if (SelectedAccount != null)
            {
                var accountUpdateFactory = SocinatorInitialize
                    .GetSocialLibrary(SelectedAccount?.AccountBaseModel?.AccountNetwork ?? SocialNetworks.Admin)
                    .GetNetworkCoreFactory().AccountUpdateFactory;
                GrowthList.Clear();
                GrowthList.AddRange(accountUpdateFactory.GetDailyGrowthForAccount(SelectedAccount?.AccountId, GrowthChartPeriods.Selected ?? GrowthChartPeriod.Past30Days));
            }
        }
    }
}
