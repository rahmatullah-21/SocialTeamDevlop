using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DominatorHouse.Model
{
    public class MainWindowModel : BindableBase
    {


        public HashSet<SocialNetworks> _availableNetworks;
        public Dock _tabDock = Dock.Left;
        public ObservableCollection<TabItemTemplates> _tabItems;
        public ObservableCollection<string> _languages;

        public Dictionary<string, CancellationToken> _accountUpdater = new Dictionary<string, CancellationToken>();

        public bool IsClickedFromMainWindow { get; set; } = true;

        public DominatorAccountViewModel.AccessorStrategies _strategies;

        private string _ramSize = string.Empty;

        public string RamSize
        {
            get
            {
                return _ramSize;
            }
            set
            {
                _ramSize = value;
                OnPropertyChanged(nameof(RamSize));
            }
        }

        private string _availablememory;

        public string Availablememory
        {
            get
            {
                return _availablememory;
            }
            set
            {
                _availablememory = value;
                OnPropertyChanged(nameof(Availablememory));
            }
        }
        private string _cpuUsage;

        public string CpuUsage
        {
            get
            {
                return _cpuUsage;
            }
            set
            {
                _cpuUsage = value;
                OnPropertyChanged(nameof(CpuUsage));
            }
        }
        private string _datetime;

        public string Datetime
        {
            get
            {
                return _datetime;
            }
            set
            {
                _datetime = value;
                OnPropertyChanged(nameof(Datetime));
            }
        }


        public ObservableCollection<string> Languages
        {
            get
            {
                return _languages;
            }
            set
            {
                _languages = value;
                OnPropertyChanged(nameof(Languages));
            }

        }
        public ObservableCollection<TabItemTemplates> TabItems
        {
            get
            {
                return _tabItems;
            }
            set
            {
                _tabItems = value;
                OnPropertyChanged(nameof(TabItems));
            }

        }
        private TabItemTemplates _selectedContent = new TabItemTemplates();

        public TabItemTemplates SelectedContent
        {
            get
            {
                return _selectedContent;
            }
            set
            {
                if (_selectedContent == value)
                    return;
                SetProperty(ref _selectedContent, value);
            }
        }

        private int _selectedViewIndex;

        public int SelectedViewIndex
        {
            get
            {
                return _selectedViewIndex;
            }
            set
            {
                _selectedViewIndex = value;
                OnPropertyChanged(nameof(SelectedViewIndex));
            }
        }

        private int _selectedNetworkIndex;

        public int SelectedNetworkIndex
        {
            get
            {
                return _selectedNetworkIndex;
            }
            set
            {
                _selectedNetworkIndex = value;
                OnPropertyChanged(nameof(SelectedNetworkIndex));
            }
        }
        private TabItem _selectedLogtab=new TabItem
        {
           Header= "Info"
        };

        public TabItem SelectedLogTab
        {
            get { return _selectedLogtab; }
            set
            {
                _selectedLogtab = value;
                OnPropertyChanged(nameof(SelectedLogTab));
            }
        }

        private static PerformanceCounter PerformanceCounter { get; }
            = new PerformanceCounter("Memory", "Available MBytes");

        private static ManagementObject Processor { get; }
            = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");

        public HashSet<SocialNetworks> AvailableNetworks
        {
            get
            {
                return _availableNetworks;
            }
            set
            {
                _availableNetworks = value;
                OnPropertyChanged(nameof(AvailableNetworks));
            }
        }

        public Dock TabDock
        {
            get
            {
                return _tabDock;
            }
            set
            {
                _tabDock = value;
                OnPropertyChanged(nameof(TabDock));
            }
        }

        #region Properties

        private ObservableCollection<LoggerModel> _lstLoggerModels = new ObservableCollection<LoggerModel>();
        public ObservableCollection<LoggerModel> LstLoggerModels
        {
            get
            {
                return _lstLoggerModels;
            }
            set
            {
                _lstLoggerModels = value;
                OnPropertyChanged(nameof(LstLoggerModels));
            }
        }
        private ObservableCollection<string> _activityType = new ObservableCollection<string>();
        public ObservableCollection<string> ActivityType
        {
            get
            {
                return _activityType;
            }
            set
            {
                _activityType = value;
                OnPropertyChanged(nameof(ActivityType));
            }
        }
        private ICollectionView _loggerCollection;

        public ICollectionView LoggerCollection
        {
            get
            {
                return _loggerCollection;
            }
            set
            {
                _loggerCollection = value;
                OnPropertyChanged(nameof(LoggerCollection));
            }
        }
        private string _selectedNetwork = string.Empty;

        public string SelectedNetwork
        {
            get
            {
                return _selectedNetwork;
            }
            set
            {
                _selectedNetwork = value;
                OnPropertyChanged(nameof(SelectedNetwork));
            }
        }
        private string _selectedActivity = string.Empty;

        public string SelectedActivity
        {
            get
            {
                return _selectedActivity;
            }
            set
            {
                _selectedActivity = value;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        #endregion

        public string LastTab { get; set; } = "Info";

    }
}
