using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
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



        private ObservableCollection<PublisherCreateDestinationSelectModel> _listSelectDestination = new ObservableCollection<PublisherCreateDestinationSelectModel>();
        [ProtoMember(10)]
        public ObservableCollection<PublisherCreateDestinationSelectModel> ListSelectDestination
        {
            get
            {
                return _listSelectDestination;
            }
            set
            {
                if (_listSelectDestination == value)
                    return;
                _listSelectDestination = value;
                OnPropertyChanged(nameof(ListSelectDestination));
            }
        }




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

        public bool UpdateDestination(PublisherCreateDestinationModel publisherCreateDestinationModel)
            => BinFileHelper.UpdateDestination(publisherCreateDestinationModel);

        public PublisherCreateDestinationModel GetDestination(string destinationId)
        {
            var publisherCreateDestinationModel = BinFileHelper.GetDestination(destinationId);
            return publisherCreateDestinationModel[0];
        }
    }

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
                UpdateGroupText();
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
                UpdateGroupText();
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
                UpdatePagesOrBoardsText();
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
                UpdatePagesOrBoardsText();
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


        private void UpdateGroupText() => 
            GroupSelectorText = IsGroupsAvailable ? SelectedGroups + "/" + TotalGroups : "NA";


        private void UpdatePagesOrBoardsText() => 
            PagesOrBoardsSelectorText = IsPagesOrBoardsAvailable ? SelectedPagesOrBoards + "/" + TotalPagesOrBoards : "NA";
    }
   
}