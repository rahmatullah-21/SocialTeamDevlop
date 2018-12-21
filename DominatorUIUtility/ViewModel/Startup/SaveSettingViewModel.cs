using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.Startup;
using Prism.Commands;

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
            LstUserControls.Add(new SelectActivity());
            LstUserControls.Add(new SearchQueryControl());
            var jobconfig = new JobConfigControl();
            JobConfiguration.RunningTime = RunningTimes.DayWiseRunningTimes;
            jobconfig.JobConfiguration = JobConfiguration;
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

        private JobConfiguration _jobConfiguration = new JobConfiguration();

        public JobConfiguration JobConfiguration
        {
            get { return _jobConfiguration; }
            set { SetProperty(ref _jobConfiguration, value); }
        }

        public void SetNetwork(SocialNetworks selectedNetwork)
        {
            var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            viewModel.SetActivityTypeByNetwork(selectedNetwork);
            SelectedNetwork = selectedNetwork;
        }
        private void OnNextClick(object sender)
        {
            var button = sender as Button;
            if (button.Content.ToString() == "Finish")
            {
                Dialog.CloseDialog(sender);
                return;
            }
            SelectedIndex += 1;
            SourceUserControl = LstUserControls[SelectedIndex];
        }

        private void OnPreviousClick(object sender)
        {
            SelectedIndex -= 1;
            SourceUserControl = LstUserControls[SelectedIndex];
        }

    }
}