using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.NetworkActivitySetting;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorHouse.ViewModels.Startup
{

    public interface ISaveSettingViewModel
    {
        void SetNetwork(string selectedNetwork);
        UserControl SourceUserControl { get; set; }
        List<UserControl> LstUserControls { get; set; }
        int SelectedIndex { get; set; }
    }
    public class SaveSettingViewModel : BindableBase, ISaveSettingViewModel
    {
        IRegionManager _region;
        public SaveSettingViewModel(IRegionManager region)
        {
            _region = region;
            NextCommand = new DelegateCommand<object>(OnNextClick);
            PreviousCommand = new DelegateCommand<object>(OnPreviousClick);
            SaveSettingModel.JobConfiguration.RunningTime = RunningTimes.DayWiseRunningTimes;

            SaveSettingModel.ListQueryType.Clear();
            SaveSettingModel.ListQueryType.Add("Query1");
            SaveSettingModel.ListQueryType.Add("Query2");
            SaveSettingModel.ListQueryType.Add("Query3");

            AddQueryCommand = new DelegateCommand<object>(AddQuery);
        }


        public ICommand AddQueryCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty(ref _selectedIndex, value); }
        }

        private UserControl _sourceUserControl;

        public UserControl SourceUserControl
        {
            get
            {
                return _sourceUserControl;
            }
            set
            {
                SetProperty(ref _sourceUserControl, value);
            }
        }

        private List<UserControl> _lstUserControls = new List<UserControl>();

        public List<UserControl> LstUserControls
        {
            get { return _lstUserControls; }
            set { SetProperty(ref _lstUserControls, value); }
        }

        private string _selectedNetwork;
        private int _selectedIndex;

        public string SelectedNetwork
        {
            get { return _selectedNetwork; }
            set { SetProperty(ref _selectedNetwork, value); }
        }


        private SaveSettingModel _saveSettingModel = new SaveSettingModel();

        public SaveSettingModel SaveSettingModel
        {
            get { return _saveSettingModel; }
            set { SetProperty(ref _saveSettingModel, value); }
        }

        public void SetNetwork(string selectedNetwork)
        {
            SetActivityTypeByNetwork(selectedNetwork);
            SelectedNetwork = selectedNetwork;
        }
        private void OnNextClick(object sender)
        {
            _region.RequestNavigate("StartupRegion", "SelectNetwork");
            string userType = string.Empty;
            string network = string.Empty;
            if (SelectedIndex == 0)
                userType = ServiceLocator.Current.GetInstance<ISelectUserTypeViewModel>().SelectedUser;
            if (SelectedIndex == 1)
            {
                network = ServiceLocator.Current.GetInstance<ISelectNetworkViewModel>().SelectedNetwork;
                SetNetwork(network);
            }

            var button = sender as Button;
            if (button.Content.ToString() == "Finish")
            {
                var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                genericFileManager.Save(SaveSettingModel, ConstantVariable.GetModuleConfigPath(SelectedNetwork.ToString()));
                Dialog.CloseDialog(sender);
                return;
            }
            //SelectedIndex += 1;
            //SourceUserControl = LstUserControls[SelectedIndex];
        }
        public void SetActivityTypeByNetwork(string network)
        {
            SaveSettingModel.LstNetworkActivityType.Clear();
            foreach (var name in Enum.GetNames(typeof(ActivityType)))
            {
                if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(network.ToString()))
                    SaveSettingModel.LstNetworkActivityType.Add(new ActivityChecked
                    {
                        ActivityType = name
                    });
            }
        }
        private void OnPreviousClick(object sender)
        {
            SelectedIndex -= 1;
            SourceUserControl = LstUserControls[SelectedIndex];
        }
        private void AddQuery(object sender)
        {
            //try
            //{
            //    var moduleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
            //    moduleSettingsUserControl?.AddQuery(typeof(FollowerQuery));
            //}
            //catch (Exception ex)
            //{
            //    ex.DebugLog();
            //}
        }
    }
}