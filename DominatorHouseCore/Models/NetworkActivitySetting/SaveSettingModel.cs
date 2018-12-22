using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.NetworkActivitySetting
{
    [ProtoContract]
    public class SaveSettingModel : BindableBase
    {
        private JobConfiguration _jobConfiguration = new JobConfiguration();
        [ProtoMember(1)]
        public JobConfiguration JobConfiguration
        {
            get { return _jobConfiguration; }
            set { SetProperty(ref _jobConfiguration, value); }
        }
        private ObservableCollection<ActivityChecked> _lstNetworkActivityType = new ObservableCollection<ActivityChecked>();
        [ProtoMember(2)]
        public ObservableCollection<ActivityChecked> LstNetworkActivityType
        {
            get { return _lstNetworkActivityType; }
            set { SetProperty(ref _lstNetworkActivityType, value); }
        }

    }
    [ProtoContract]
    public class SelectActivityModel : BindableBase
    {

        private ObservableCollection<ActivityChecked> _lstNetworkActivityType = new ObservableCollection<ActivityChecked>();
        [ProtoMember(1)]
        public ObservableCollection<ActivityChecked> LstNetworkActivityType
        {
            get { return _lstNetworkActivityType; }
            set { SetProperty(ref _lstNetworkActivityType, value); }
        }


    }
    [ProtoContract]
    public class ActivityChecked : BindableBase
    {
        private string _activityType;
        [ProtoMember(1)]
        public string ActivityType
        {
            get { return _activityType; }
            set { SetProperty(ref _activityType, value); }
        }
        private bool _isActivity;
        [ProtoMember(2)]
        public bool IsActivity
        {
            get { return _isActivity; }
            set { SetProperty(ref _isActivity, value); }
        }

    }

}
