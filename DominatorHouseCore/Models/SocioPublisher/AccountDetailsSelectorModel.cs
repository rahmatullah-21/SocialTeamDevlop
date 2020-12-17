#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class AccountDetailsSelectorModel : BindableBase
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex == value)
                    return;
                _currentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
            }
        }


        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
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
            get => _detailName;
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
            get => _detailUrl;
            set
            {
                if (_detailUrl == value)
                    return;
                _detailUrl = value;
                OnPropertyChanged(nameof(DetailUrl));
            }
        }

        private bool _isOwnPage;

        public bool IsOwnPage
        {
            get => _isOwnPage;
            set
            {
                if (_isOwnPage == value)
                    return;
                _isOwnPage = value;
                OnPropertyChanged(nameof(IsOwnPage));
            }
        }


        private bool _isLikePage;

        public bool IsLikePage
        {
            get => _isLikePage;
            set
            {
                if (_isLikePage == value)
                    return;
                _isLikePage = value;
                OnPropertyChanged(nameof(IsLikePage));
            }
        }


        private bool _isFanpage;

        public bool IsFanpage
        {
            get => _isFanpage;
            set
            {
                if (_isFanpage == value)
                    return;
                _isFanpage = value;
                OnPropertyChanged(nameof(IsFanpage));
            }
        }

        private bool _isGroup;

        public bool IsGroup
        {
            get => _isGroup;
            set
            {
                if (_isGroup == value)
                    return;
                _isGroup = value;
                OnPropertyChanged(nameof(IsGroup));
            }
        }

        private bool _isOwnGroup;

        public bool IsOwnGroup
        {
            get => _isOwnGroup;
            set
            {
                if (_isOwnGroup == value)
                    return;
                _isOwnGroup = value;
                OnPropertyChanged(nameof(IsOwnGroup));
            }
        }


        private bool _isJoinedGroup;

        public bool IsJoinedGroup
        {
            get => _isJoinedGroup;
            set
            {
                if (_isJoinedGroup == value)
                    return;
                _isJoinedGroup = value;
                OnPropertyChanged(nameof(IsJoinedGroup));
            }
        }

        public SocialNetworks Network { get; set; }
    }
}