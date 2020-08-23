using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.Models.NetworkActivitySetting
{
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

}
