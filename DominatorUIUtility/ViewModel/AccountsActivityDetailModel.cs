using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System.Text.RegularExpressions;

namespace DominatorUIUtility.ViewModel
{
    public class AccountsActivityDetailModel : BindableBase
    {
        public string AccountName { get; set; }

        public string AccountId { get; set; }

        public SocialNetworks AccountNetwork { get; set; }

        public ObservableCollection<ActivityDetailsModel> ActivityDetailsCollections { get; set; }

        string _showMoreButtonText = "LangKeyMore".FromResourceDictionary();
        public string ShowMoreButtonText { get=> _showMoreButtonText; set { SetProperty(ref _showMoreButtonText, value); } }
    }

    public class ActivityDetailsModel : BindableBase
    {

        private string _accountId = string.Empty;

        public string AccountId
        {
            get
            {
                return _accountId;
            }
            set
            {
                if (_accountId == value)
                    return;
                SetProperty(ref _accountId, value);
            }
        }

        private ActivityType _title;
        public ActivityType Title
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
        private string _activityTitle;

        public string ActivityTitle
        {
            get
            {
                _activityTitle = Regex.Replace(Title.ToString(), "(\\B[A-Z])", " $1");
                return _activityTitle;
            }
            set
            {
                if (_activityTitle == value)
                    return;
                SetProperty(ref _activityTitle, value);
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
