using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Text.RegularExpressions;

namespace DominatorHouseCore.Models.NetworkActivitySetting
{
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


        public string DisplayActivity
        {
            get
            {
                return Regex.Replace(ActivityType, "(\\B[A-Z])", " $1");
            }
        }

    }
}
