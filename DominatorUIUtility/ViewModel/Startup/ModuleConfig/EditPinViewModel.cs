using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LegionUIUtility.ViewModel.Startup.ModuleConfig
{
    public class PinInfo : BindableBase
    {
        private string _board;

        public string Board
        {
            get
            {
                return _board;
            }
            set
            {
                if (_board != null && _board == value)
                    return;
                SetProperty(ref _board, value);
            }
        }

        private string _pinDescription;

        public string PinDescription
        {
            get
            {
                return _pinDescription;
            }
            set
            {
                if (_pinDescription != null && _pinDescription == value)
                    return;
                SetProperty(ref _pinDescription, value);
            }
        }


        private string _section;

        public string Section
        {
            get
            {
                return _section;
            }
            set
            {
                if (_section != null && _section == value)
                    return;
                SetProperty(ref _section, value);
            }
        }

        private string _websiteUrl;

        public string WebsiteUrl
        {
            get
            {
                return _websiteUrl;
            }
            set
            {
                if (_websiteUrl != null && _websiteUrl == value)
                    return;
                SetProperty(ref _websiteUrl, value);
            }
        }

        private string _pinToBeEdit;

        public string PinToBeEdit
        {
            get
            {
                return _pinToBeEdit;
            }
            set
            {
                if (_pinToBeEdit != null && _pinToBeEdit == value)
                    return;
                SetProperty(ref _pinToBeEdit, value);
            }
        }

        private string _account;

        public string Account
        {
            get
            {
                return _account;
            }
            set
            {
                if (_account != null && _account == value)
                    return;
                SetProperty(ref _account, value);
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex != 0 && _selectedIndex == value)
                    return;
                SetProperty(ref _selectedIndex, value);
            }
        }

        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
    }
    public interface IEditPinViewModel
    {
    }
    public class EditPinViewModel : StartupBaseViewModel, IEditPinViewModel
    {
        public EditPinViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.EditPin });
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfEditPinsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfEditPinsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfEditPinsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfEditPinsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyEditPinsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddPinCommand = new BaseCommand<object>((sender) => true, AddPin);
            DeletePinCommand = new BaseCommand<object>((sender) => true, DeletePin);
            ImportFromCsvCommand = new BaseCommand<object>((sender) => true, ImportFromCsv);
        }

        private void ValidateAndNevigate()
        {
            if (PinDetails.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one pin.");
                return;
            }
            else
                NavigateNext();
        }

        public ICommand AddPinCommand { get; set; }
        public ICommand DeletePinCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }

        private void AddPin(object sender)
        {
            try
            {
                if (PinDetails.Count == 0)
                    PinDetails = new ObservableCollectionBase<PinInfo>();
                var editPinControl = sender as PinInfo;
                var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
                editPinControl.Account = viewModel.SelectAccount.AccountBaseModel.UserName;

                editPinControl.PinToBeEdit = editPinControl.PinToBeEdit.Trim();
                editPinControl.Account = editPinControl.Account.Trim();
                editPinControl.Board = editPinControl.Board.Trim();
                editPinControl.PinDescription = editPinControl.PinDescription.Trim();
                editPinControl.Section = editPinControl.Section.Trim();
                editPinControl.WebsiteUrl = editPinControl.WebsiteUrl.Trim();



                if (string.IsNullOrEmpty(editPinControl.PinToBeEdit) || string.IsNullOrEmpty(editPinControl.Account) ||
                (string.IsNullOrEmpty(editPinControl.Board) && string.IsNullOrEmpty(editPinControl.PinDescription) &&
                string.IsNullOrEmpty(editPinControl.Section) && string.IsNullOrEmpty(editPinControl.WebsiteUrl)))
                    return;
                if (listDetails.Count > 0)
                {
                    listDetails.ForEach(pin =>
                    {
                        if (!PinDetails.Any(info => (info.Account == pin.Account && info.PinToBeEdit.ToLower() == "all") ||
                        info.PinToBeEdit.Contains(editPinControl.PinToBeEdit)))
                        {
                            PinDetails.Add(new PinInfo()
                            {
                                Board = pin.Board,
                                PinDescription = pin.PinDescription,
                                Section = pin.Section,
                                WebsiteUrl = pin.WebsiteUrl,
                                PinToBeEdit = pin.PinToBeEdit,
                                Account = pin.Account
                            });
                        }
                    });
                }
                else if (!PinDetails.Any(info => (info.Account == editPinControl.Account && info.PinToBeEdit.ToLower() == "all")
                || info.PinToBeEdit.Contains(editPinControl.PinToBeEdit)))
                {
                    PinDetails.Add(new PinInfo()
                    {
                        Board = editPinControl.Board,
                        PinDescription = editPinControl.PinDescription,
                        Section = editPinControl.Section,
                        WebsiteUrl = editPinControl.WebsiteUrl,
                        PinToBeEdit = editPinControl.PinToBeEdit,
                        Account = editPinControl.Account
                    });
                }
                else
                    return;

                editPinControl.Board = string.Empty;
                editPinControl.PinDescription = string.Empty;
                editPinControl.Section = string.Empty;
                editPinControl.WebsiteUrl = string.Empty;
                editPinControl.PinToBeEdit = string.Empty;
                //editPinControl.Account = string.Empty;
                editPinControl.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeletePin(object sender)
        {
            try
            {
                var pindetails = sender as PinInfo;
                PinDetails.Remove(pindetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ImportFromCsv(object sender)
        {
            try
            {
                //listDetails.Clear();
                //listPins.Clear();
                //listPins.AddRange(FileUtilities.FileBrowseAndReader());
                //var accountFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
                //var accounts = accountFileManager.GetAll(SocialNetworks.Pinterest).Where(x => x.AccountBaseModel.Status == AccountStatus.Success).
                //    Select(x => x.AccountBaseModel.UserName).ToList();

                //if (listPins.Count != 0)
                //{
                //    foreach (string pin in listPins)
                //    {
                //        try
                //        {
                //            string[] pindetails = pin.Split('\t');
                //            if (pindetails[0] == "Board")
                //            {
                //                continue;
                //            }
                //            PinInfo pinInfo = new PinInfo();
                //            pinInfo.Board = pindetails[0];
                //            pinInfo.PinDescription = pindetails[1];
                //            pinInfo.Section = pindetails[2];
                //            pinInfo.WebsiteUrl = pindetails[3];
                //            pinInfo.PinToBeEdit = pindetails[4];
                //            pinInfo.Account = pindetails[5];

                //            if (!accounts.Contains(pinInfo.Account))
                //            {
                //                GlobusLogHelper.log.Info($"Account => {pinInfo.Account} is not present.");
                //                continue;
                //            }

                //            listDetails.Add(pinInfo);
                //        }
                //        catch (Exception ex)
                //        {
                //            ex.DebugLog();
                //        }
                //    }
                //    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Info",
                //        "Pins are ready to add !!");
                //    GlobusLogHelper.log.Info("Pins sucessfully uploaded !!");
                //}
                //else
                //    GlobusLogHelper.log.Info("You did not upload any pins !!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("There is error in uploading pins !!");
            }
        }

        private ObservableCollectionBase<PinInfo> _pinDetails = new ObservableCollectionBase<PinInfo>();
        public ObservableCollectionBase<PinInfo> PinDetails
        {
            get
            {
                return _pinDetails;
            }
            set
            {
                if (_pinDetails != null && _pinDetails == value)
                    return;
                SetProperty(ref _pinDetails, value);
            }
        }

        private PinInfo _currentPin = new PinInfo();
        public PinInfo CurrentPin
        {
            get
            {
                return _currentPin;
            }
            set
            {
                if (_currentPin != null && _currentPin == value)
                    return;
                SetProperty(ref _currentPin, value);
            }
        }

        private List<PinInfo> _listPinsDetails = new List<PinInfo>();
        public List<PinInfo> listDetails
        {
            get
            {
                return _listPinsDetails;
            }
            set
            {
                if (_listPinsDetails != null && _listPinsDetails == value)
                    return;
                SetProperty(ref _listPinsDetails, value);
            }
        }

        private List<string> _listPins = new List<string>();
        public List<string> listPins
        {
            get
            {
                return _listPins;
            }
            set
            {
                if (_listPins != null && _listPins == value)
                    return;
                SetProperty(ref _listPins, value);
            }
        }
    }
}
