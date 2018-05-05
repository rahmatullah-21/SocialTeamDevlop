using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherCreateDestinationSelectModel : BindableBase
    {
        private bool _isAccountSelected;
        [ProtoMember(1)]
        public bool IsAccountSelected
        {
            get
            {
                return _isAccountSelected;
            }
            set
            {
                if(_isAccountSelected==value)
                    return;
                _isAccountSelected = value;
                OnPropertyChanged(nameof(IsAccountSelected));
            }
        }


        private string _accountId;
        [ProtoMember(2)]

        public string AccountId
        {
            get
            {
                return _accountId;
            }
            set
            {
                _accountId = value;
                OnPropertyChanged(nameof(AccountId));
            }
        }

        private string _accountName;
        [ProtoMember(3)]

        public string AccountName
        {
            get
            {
                return _accountName;
            }
            set
            {
                _accountName = value;
                OnPropertyChanged(nameof(AccountName));
            }
        }

        [ProtoMember(4)]

        public SocialNetworks SocialNetworks { get; set; }

        [ProtoMember(5)]

        public bool IsGroupsAvailable { get; set; }

        [ProtoMember(6)]

        public bool IsPagesOrBoardsAvailable { get; set; }


        private string _groupSelectorText = "NA";
        [ProtoMember(7)]
        public string GroupSelectorText
        {
            get
            {
                return _groupSelectorText;
            }
            set
            {
                if (_groupSelectorText == value)
                    return;
                _groupSelectorText = value;
                OnPropertyChanged(nameof(GroupSelectorText));
            }
        }


        private string _pagesOrBoardsSelectorText = "NA";
        [ProtoMember(8)]
        public string PagesOrBoardsSelectorText
        {
            get
            {
                return _pagesOrBoardsSelectorText;
            }
            set
            {
                if (_pagesOrBoardsSelectorText == value)
                    return;
                _pagesOrBoardsSelectorText = value;
                OnPropertyChanged(nameof(PagesOrBoardsSelectorText));
            }
        }

        private int _totalGroups;
        [ProtoMember(9)]
        public int TotalGroups
        {
            get
            {
                return _totalGroups;
            }
            set
            {
                if(_totalGroups == value)
                    return;
                _totalGroups = value;
                // UpdateGroupText();
                OnPropertyChanged(nameof(TotalGroups));
            }
        }

        private int _selectedGroups;
        [ProtoMember(10)]

        public int SelectedGroups
        {
            get
            {
                return _selectedGroups;
            }
            set
            {
                if (_selectedGroups == value)
                    return;
                _selectedGroups = value;
                // UpdateGroupText();
                OnPropertyChanged(nameof(SelectedGroups));
            }
        }


        private int _totalPagesOrBoards;
        [ProtoMember(11)]

        public int TotalPagesOrBoards
        {
            get
            {
                return _totalPagesOrBoards;
            }
            set
            {
                if (_totalPagesOrBoards == value)
                    return;
                _totalPagesOrBoards = value;           
                OnPropertyChanged(nameof(TotalPagesOrBoards));
            }
        }

        private int _selectedPagesOrBoards;
        [ProtoMember(12)]

        public int SelectedPagesOrBoards
        {
            get
            {
                return _selectedPagesOrBoards;
            }
            set
            {
                if (_selectedPagesOrBoards == value)
                    return;
                _selectedPagesOrBoards = value;           
                OnPropertyChanged(nameof(SelectedPagesOrBoards));

            }
        }

        private bool _publishonOwnWall;
        [ProtoMember(13)]

        public bool PublishonOwnWall
        {
            get
            {
                return _publishonOwnWall;
            }
            set
            {
                if(_publishonOwnWall == value)
                    return;
                _publishonOwnWall = value;
                OnPropertyChanged(nameof(PublishonOwnWall));
            }
        }

        public void UpdateGroupText() => 
            GroupSelectorText = IsGroupsAvailable ? SelectedGroups + "/" + TotalGroups : "NA";

        public void UpdatePagesOrBoardsText() => 
            PagesOrBoardsSelectorText = IsPagesOrBoardsAvailable ? SelectedPagesOrBoards + "/" + TotalPagesOrBoards : "NA";
    }
}