using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class AccountDetailsSelectorModel : BindableBase
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }


        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private string _detailName;

        public string DetailName
        {
            get
            {
                return _detailName;
            }
            set
            {
                if (_detailName == value)
                    return;
                _detailName = value;
                OnPropertyChanged(nameof(DetailName));
            }
        }


        private string _detailUrl;

        public string DetailUrl
        {
            get
            {
                return _detailUrl;
            }
            set
            {
                if (_detailUrl == value)
                    return;
                _detailUrl = value;
                OnPropertyChanged(nameof(DetailUrl));
            }
        }

        public SocialNetworks Network { get; set; }
    }
}