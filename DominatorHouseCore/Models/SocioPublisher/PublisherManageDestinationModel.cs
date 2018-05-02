using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherManageDestinationModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _accountCount;
        private int _pagesOrBoardsCount;
        private int _groupsCount;
        private int _wallsOrProfilesCount;
        private int _campaignsCount;

        [ProtoMember(1)]
        public string DestinationId { get; set; }

        [ProtoMember(2)]
        public string DestinationName { get; set; }

        [ProtoMember(3)]
        public int AccountCount
        {
            get
            {
                return _accountCount;
            }
            set
            {
                if (_accountCount == value)
                    return;
                _accountCount = value;
                OnPropertyChanged(nameof(AccountCount));
            }
        }

        [ProtoMember(4)]
        public int PagesOrBoardsCount
        {
            get
            {
                return _pagesOrBoardsCount;
            }
            set
            {
                if (_pagesOrBoardsCount == value)
                    return;
                _pagesOrBoardsCount = value;
                OnPropertyChanged(nameof(PagesOrBoardsCount));
            }
        }

        [ProtoMember(5)]
        public int GroupsCount
        {
            get
            {
                return _groupsCount;
            }
            set
            {
                if (_groupsCount == value)
                    return;
                _groupsCount = value;
                OnPropertyChanged(nameof(GroupsCount));
            }
        }

        [ProtoMember(6)]
        public int WallsOrProfilesCount
        {
            get
            {
                return _wallsOrProfilesCount;
            }
            set
            {
                if (_wallsOrProfilesCount == value)
                    return;
                _wallsOrProfilesCount = value;
                OnPropertyChanged(nameof(WallsOrProfilesCount));
            }
        }

        [ProtoMember(7)]
        public int CampaignsCount
        {
            get
            {
                return _campaignsCount;
            }
            set
            {
                if (_campaignsCount == value)
                    return;
                _campaignsCount = value;
                OnPropertyChanged(nameof(CampaignsCount));
            }
        }

        [ProtoMember(8)]
        public DateTime CreatedDate { get; set; }




        [ProtoIgnore]
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

        public void GenerateDestinations()
        {
            DestinationName = $"Destination-{ConstantVariable.GetDateTime()}";
            DestinationId = Utilities.GetGuid();
            CreatedDate = DateTime.Today;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}