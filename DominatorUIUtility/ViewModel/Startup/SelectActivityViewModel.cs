using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface ISelectActivityViewModel
    {
        void SetActivityTypeByNetwork(SocialNetworks network);
    }
    public class SelectActivityViewModel : BindableBase, ISelectActivityViewModel
    {
        private ObservableCollection<ActivityChecked> _lstNetworkActivityType = new ObservableCollection<ActivityChecked>();

        public ObservableCollection<ActivityChecked> LstNetworkActivityType
        {
            get { return _lstNetworkActivityType; }
            set { SetProperty(ref _lstNetworkActivityType, value); }
        }

        public void SetActivityTypeByNetwork(SocialNetworks network)
        {
            LstNetworkActivityType.Clear();
            foreach (var name in Enum.GetNames(typeof(ActivityType)))
            {
                if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(network.ToString()))
                    LstNetworkActivityType.Add(new ActivityChecked
                    {
                        ActivityType = name
                    });
            }
        }
    }

    public class ActivityChecked : BindableBase
    {
        private string _activityType;

        public string ActivityType
        {
            get { return _activityType; }
            set { SetProperty(ref _activityType, value); }
        }
        private bool _isActivity;

        public bool IsActivity
        {
            get { return _isActivity; }
            set { SetProperty(ref _isActivity, value); }
        }

    }
}
