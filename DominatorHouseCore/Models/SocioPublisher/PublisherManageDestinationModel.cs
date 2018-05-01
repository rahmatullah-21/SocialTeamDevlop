using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherManageDestinationModel : INotifyPropertyChanged
    {
        [ProtoMember(1)]
        public string DestinationId { get; set; }

        [ProtoMember(2)]
        public string DestinationName { get; set; }

        [ProtoMember(3)]
        public int AccountCount { get; set; }

        [ProtoMember(4)]
        public int PagesOrBoardsCount { get; set; }

        [ProtoMember(5)]
        public int GroupsCount { get; set; }

        [ProtoMember(6)]
        public int WallsOrProfilesCount { get; set; }

        [ProtoMember(7)]
        public int  CampaignsCount { get; set; }

        [ProtoMember(8)]
        public DateTime CreatedDate { get; set; }

        [ProtoMember(9)]
        public bool IsRemoveGroupsRequiresApproval { get; set; }

        [ProtoMember(10)]
        public bool IsAddedNewGroups { get; set; }

        [ProtoMember(11)]
        public List<string> SelectedAccountId { get; set; }

        [ProtoMember(12)]
        public List<string> SelectedGroupUrls { get; set; }

        [ProtoMember(13)]
        public List<string> SelectedPagesOrBoardsUrls { get; set; }

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

    public class DestinationCollection : BindableBase
    {
        private bool _isAccountSelected;
        private int _selectedGroupCount;

        public bool IsAccountSelected
        {
            get
            {
                return _isAccountSelected;
            }
            set
            {
                _isAccountSelected = value;
                OnPropertyChanged(nameof(IsAccountSelected));
            }
        }

        public string AccountName { get; set; }

        public int SelectedGroupCount
        {
            get
            {
                return _selectedGroupCount;
            }
            set
            {
                _selectedGroupCount = value;
                OnPropertyChanged(nameof(SelectedGroupCount));
            }
        }

        public int TotalGroupCount { get; set; }

        public int SelectedBoardOrPageCount { get; set; }

        public int TotalBoardOrPageCount { get; set; }

        public bool IsWallOrProfileSelected { get; set; }
    }
}