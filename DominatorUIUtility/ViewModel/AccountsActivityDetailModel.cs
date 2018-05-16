using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;

namespace DominatorUIUtility.ViewModel
{
    public class AccountsActivityDetailModel :BindableBase
    {
        public string AccountName { get; set; }

        public string AccountId { get; set; }

        public SocialNetworks  AccountNetwork { get; set; }

        public ObservableCollection<ActivityDetailsModel> ActivityDetailsCollections { get; set; }
    }

    public class ActivityDetailsModel : BindableBase
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value)
                    return;
                SetProperty(ref _title, value);
            }
        }

        private bool _status;
        public bool Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status == value)
                    return;
                SetProperty(ref _status, value);
            }
        }
    }

  

}
