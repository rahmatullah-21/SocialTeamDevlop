using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class PublisherCreateDestinationModel : INotifyPropertyChanged
    {
        private List<KeyValuePair<string, string>> _accountPagesBoardsPair = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _accountGroupPair = new List<KeyValuePair<string, string>>();
        private List<string> _selectedAccountIds = new List<string>();
        private List<string> _publishOwnWallAccount = new List<string>();
        private bool _isRemoveGroupsRequiresApproval;
        private bool _isAddedNewGroups;
        private string _destinationId;
        private string _destinationName;

        [ProtoMember(1)]
        public string DestinationId
        {
            get
            {
                return _destinationId;
            }
            set
            {
                if (_destinationId == value)
                    return;
                _destinationId = value;
                OnPropertyChanged(nameof(DestinationId));
            }
        }


        [ProtoMember(2)]
        public string DestinationName
        {
            get
            {
                return _destinationName;
            }
            set
            {
                if (_destinationName == value)
                    return;
                _destinationName = value;
                OnPropertyChanged(nameof(DestinationName));
            }
        }

        [ProtoMember(3)]
        public bool IsRemoveGroupsRequiresApproval
        {
            get
            {
                return _isRemoveGroupsRequiresApproval;
            }
            set
            {
                if (_isRemoveGroupsRequiresApproval == value)
                    return;
                _isRemoveGroupsRequiresApproval = value;
                OnPropertyChanged(nameof(IsRemoveGroupsRequiresApproval));
            }
        }

        [ProtoMember(4)]
        public bool IsAddedNewGroups
        {
            get
            {
                return _isAddedNewGroups;
            }
            set
            {
                if (_isAddedNewGroups == value)
                    return;
                _isAddedNewGroups = value;
                OnPropertyChanged(nameof(IsAddedNewGroups));
            }
        }


        [ProtoMember(5)]
        public List<KeyValuePair<string, string>> AccountPagesBoardsPair
        {
            get
            {
                return _accountPagesBoardsPair;
            }
            set
            {
                if (_accountPagesBoardsPair == value)
                    return;
                _accountPagesBoardsPair = value;
                OnPropertyChanged(nameof(AccountPagesBoardsPair));
            }
        }

        [ProtoMember(6)]
        public List<KeyValuePair<string, string>> AccountGroupPair
        {
            get
            {
                return _accountGroupPair;
            }
            set
            {
                if (_accountGroupPair == value)
                    return;
                _accountGroupPair = value;
                OnPropertyChanged(nameof(AccountGroupPair));
            }
        }

        [ProtoMember(7)]
        public List<string> SelectedAccountIds
        {
            get
            {
                return _selectedAccountIds;
            }
            set
            {
                if (_selectedAccountIds == value)
                    return;
                _selectedAccountIds = value;
                OnPropertyChanged(nameof(SelectedAccountIds));
            }
        }

        [ProtoMember(8)]
        public List<string> PublishOwnWallAccount
        {
            get
            {
                return _publishOwnWallAccount;
            }
            set
            {
                if (_publishOwnWallAccount == value)
                    return;
                _publishOwnWallAccount = value;
                OnPropertyChanged(nameof(PublishOwnWallAccount));
            }
        }

        [ProtoMember(9)]
        public DateTime CreatedDate { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static PublisherCreateDestinationModel DestinationDefaultBuilder()
            => new PublisherCreateDestinationModel
            {
                DestinationName = $"Default-{ConstantVariable.GetDateTime()}",
                DestinationId = Utilities.GetGuid(),
                AccountPagesBoardsPair = new List<KeyValuePair<string, string>>(),
                AccountGroupPair = new List<KeyValuePair<string, string>>(),
                PublishOwnWallAccount = new List<string>(),
                SelectedAccountIds = new List<string>(),
                CreatedDate = DateTime.Now
            };
    
        public bool AddDestination(PublisherCreateDestinationModel publisherCreateDestinationModel) 
            => BinFileHelper.AddDestination(publisherCreateDestinationModel);

        public static PublisherCreateDestinationModel GetDestination(string destinationId)
            => BinFileHelper.GetDestination(destinationId)[0];
    }

    public class PublisherCreateDestinationSelectModel : BindableBase
    {
        private bool _isAccountSelected;

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

        public SocialNetworks SocialNetworks { get; set; }

        public bool IsGroupsAvailable { get; set; }

        public bool IsPagesOrBoardsAvailable { get; set; }


        private string _groupSelectorText = "NA";

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
                UpdateGroupText();
                OnPropertyChanged(nameof(TotalGroups));
            }
        }

        private int _selectedGroups;

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
                UpdateGroupText();
                OnPropertyChanged(nameof(SelectedGroups));
            }
        }


        private int _totalPagesOrBoards;

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
                UpdatePagesOrBoardsText();
                OnPropertyChanged(nameof(TotalPagesOrBoards));
            }
        }

        private int _selectedPagesOrBoards;

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
                UpdatePagesOrBoardsText();
                OnPropertyChanged(nameof(SelectedPagesOrBoards));

            }
        }

        private bool _publishonOwnWall;

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


        private void UpdateGroupText() => 
            GroupSelectorText = IsGroupsAvailable ? SelectedGroups + "/" + TotalGroups : "NA";


        private void UpdatePagesOrBoardsText() => 
            PagesOrBoardsSelectorText = IsPagesOrBoardsAvailable ? SelectedPagesOrBoards + "/" + TotalPagesOrBoards : "NA";
    }
   
}