using System;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.NetworkActivitySetting;
using DominatorHouse.ViewModels.Startup;
using Prism.Regions;
using Prism.Commands;
using System.Linq;
using DominatorHouseCore.Utility;
using DominatorHouse.ViewModels.Startup.ModuleConfig;
using CommonServiceLocator;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface ISelectActivityViewModel
    {
        void SetActivityTypeByNetwork(string network);
        SelectActivityModel SelectActivityModel { get; set; }
    }
    public class SelectActivityViewModel : StartupBaseViewModel, ISelectActivityViewModel
    {
        public SelectActivityViewModel(IRegionManager region) : base(region)
        {

            NextCommand = new DelegateCommand<string>(OnNextClick);
            PreviousCommand = new DelegateCommand<string>(OnPreviousClick);
        }

        private SelectActivityModel _selectActivityModel = new SelectActivityModel();

        public SelectActivityModel SelectActivityModel
        {
            get { return _selectActivityModel; }
            set { SetProperty(ref _selectActivityModel, value); }
        }
        private new void OnNextClick(string sender)
        {
            var selectActivity = CommonServiceLocator.ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            var allSelectedActivity = selectActivity.SelectActivityModel.LstNetworkActivityType.Where(x => x.IsActivity);
            if (allSelectedActivity.Count() == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one activity.");
                return;
            }

            var jobConfigViewModel = ServiceLocator.Current.GetInstance<IJobConfigViewModel>();
            jobConfigViewModel.AddJobConfiguration(allSelectedActivity);

            base.OnNextClick(sender);

        }
        public void SetActivityTypeByNetwork(string network)
        {
            SelectActivityModel.LstNetworkActivityType.Clear();
            foreach (var name in Enum.GetNames(typeof(ActivityType)))
            {
                if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(network))
                    SelectActivityModel.LstNetworkActivityType.Add(new ActivityChecked
                    {
                        ActivityType = name
                    });
            }
        }
    }
}
