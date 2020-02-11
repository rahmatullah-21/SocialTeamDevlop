using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class SoftwareSettingsViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly ISoftwareSettings _softwareSettings;

        public SoftwareSettingsModel SoftwareSettingsModel { get; }

        public DelegateCommand SaveCmd { get; }

        public DelegateCommand ExportCommand { get; }

        public SoftwareSettingsViewModel(ISoftwareSettings softwareSettings) : base("LangKeySoftwareSettings", "SoftwareSettingsControlTemplate")
        {
            _softwareSettings = softwareSettings;
            SaveCmd = new DelegateCommand(Save);
            SoftwareSettingsModel = softwareSettings.Settings;
            ExportCommand = new DelegateCommand(Export);

            //Assign LocationDetails
            SoftwareSettingsModel.ListLocationModelTemp = SoftwareSettingsModel.ListLocationModel = softwareSettings.AssignLocationList();

            SoftwareSettingsModel.DebugVisibility = Visibility.Collapsed;

            #if DEBUG
                SoftwareSettingsModel.DebugVisibility = Visibility.Visible;
            #endif
        }

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                if (value == _searchText)
                    return;
                SetProperty(ref _searchText, value);
                SetListLocationModel(value);
            }
        }
        public void SetListLocationModel(string input)
        {
            SoftwareSettingsModel.ListLocationModel.ForEach
                (x => x = SoftwareSettingsModel.ListLocationModelTemp.FirstOrDefault(y => y.CountryName == x.CountryName));

            SoftwareSettingsModel.ListLocationModelTemp = new ObservableCollection<LocationModel>
            (SoftwareSettingsModel.ListLocationModel.ToList().Where(x => x.CountryName.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)));
        }

        private void Export()
        {
            SoftwareSettingsModel.ExportPath = FileUtilities.GetExportPath(true);
        }
        private bool _progressRing = false;
        public bool ProgressRing
        {
            get { return _progressRing; }
            set
            {
                if (_progressRing == value) return;
                SetProperty(ref _progressRing, value);
            }
        }
        private void Save()
        {
            ProgressRing = true;

            if (SoftwareSettingsModel.IsSelectCountriesFilter)
            {
                try
                {

                    IGlobalDatabaseConnection dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                    var dbGlobalContext = dataBaseConnectionGlb.GetSqlConnection();
                    var _dbGlobalListOperations = new DbOperations(dbGlobalContext);

                    var ListCountry = _dbGlobalListOperations.Get<DominatorHouseCore.DatabaseHandler.DHTables.LocationList>();
                    var dt = new List<DominatorHouseCore.DatabaseHandler.DHTables.LocationList>();
                    foreach (var locationModel in SoftwareSettingsModel.ListLocationModel.Where(x => x.IsSelected))
                    {
                        if (ListCountry.Any(x => x.CountryName.Equals(locationModel.CountryName)))
                            continue;

                        var request = (HttpWebRequest)WebRequest.Create($"http://209.250.252.53/DownloadForSocinator/CityListByCountries/{locationModel.CountryName}.txt");
                        var response = request.GetResponse();
                        string cityResponse = string.Empty;
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                var reader = new StreamReader(responseStream, Encoding.UTF8);
                                cityResponse = reader.ReadToEnd();
                            }
                        }

                        List<string> cityList = System.Text.RegularExpressions.Regex.Split(cityResponse, "\r\n").ToList();
                        cityList.ForEach(x =>
                        {
                            var lst = new DominatorHouseCore.DatabaseHandler.DHTables.LocationList()
                            {
                                CountryName = locationModel.CountryName,
                                CityName = x,
                                IsSelected = false
                            };
                            dt.Add(lst);
                        });
                    }
                    _dbGlobalListOperations.AddRange(dt);
                    ToasterNotification.ShowSuccess("Location Details Downloaded Successfully");
                }
                catch (WebException)
                {
                    ToasterNotification.ShowError("failed To Downloaded Location Details");
                }
                catch (Exception)
                {
                    ToasterNotification.ShowError("failed To Downloaded Location Details");
                }

            }

            ProgressRing = false;

            if (SoftwareSettingsModel.IsDefaultExportPathSelected)
            {
                if (!string.IsNullOrEmpty(SoftwareSettingsModel.ExportPath) && Directory.Exists(SoftwareSettingsModel.ExportPath))
                    SaveSetting();
                else
                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(), "LangKeyEnterValidFolderPath".FromResourceDictionary());
            }
            else
            {
                SoftwareSettingsModel.ExportPath = string.Empty;
                SaveSetting();
            }

        }

        private void SaveSetting()
        {
            if (_softwareSettings.Save())
            {
                var result = Dialog.ShowCustomDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyConfirmToRestartAfterSoftwareSettingSaved".FromResourceDictionary(),
                    "LangKeyRestartNow".FromResourceDictionary(), "LangKeyRestartLater".FromResourceDictionary());
                if (result == MessageDialogResult.Affirmative)
                {
                    Application.Current.Shutdown();
                    Process.Start(Application.ResourceAssembly.Location);
                    Process.GetCurrentProcess().Kill();
                    Environment.Exit(0);
                }
            }
        }
    }
}
