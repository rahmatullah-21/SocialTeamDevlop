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
using DominatorUIUtility.CustomControl;
using Prism.Commands;
using DominatorUIUtility.Views.Startup;

namespace DominatorUIUtility.ViewModel.Startup
{

    public interface ISaveSettingViewModel
    {
        void SetNetwork(SocialNetworks selectedNetwork);
        UserControl SourceUserControl { get; set; }
        List<UserControl> LstUserControls { get; set; }
        int SelectedIndex { get; set; }
    }
    public class SaveSettingViewModel : BindableBase, ISaveSettingViewModel
    {
        public SaveSettingViewModel()
        {
            NextCommand = new DelegateCommand<object>(OnNextClick);
            PreviousCommand = new DelegateCommand<object>(OnPreviousClick);
            LstUserControls.Add(new SelectUserType());
            LstUserControls.Add(new SelectNetwork());
            LstUserControls.Add(new SelectActivity(this));
            LstUserControls.Add(new SearchQueryControl());
            var jobconfig = new JobConfigControl();
            SaveSettingModel.JobConfiguration.RunningTime = RunningTimes.DayWiseRunningTimes;
            jobconfig.JobConfiguration = SaveSettingModel.JobConfiguration;
            LstUserControls.Add(jobconfig);
        }


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

        private SocialNetworks _selectedNetwork;
        private int _selectedIndex;

        public SocialNetworks SelectedNetwork
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

        public void SetNetwork(SocialNetworks selectedNetwork)
        {
            SetActivityTypeByNetwork(selectedNetwork);
            SelectedNetwork = selectedNetwork;
        }
        private void OnNextClick(object sender)
        {
            var button = sender as Button;
            if (button.Content.ToString() == "Finish")
            {
                var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                genericFileManager.Save(SaveSettingModel,ConstantVariable.GetModuleConfigPath(SelectedNetwork.ToString()));
                Dialog.CloseDialog(sender);
                return;
            }
            SelectedIndex += 1;
            SourceUserControl = LstUserControls[SelectedIndex];
        }
        public void SetActivityTypeByNetwork(SocialNetworks network)
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

    }
}