using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublisherCampaignStatusModel : INotifyPropertyChanged
    {
        public string CampaignId { get; set; } = string.Empty;

        private bool _isSelected;
        private int _destinationCount;
        private int _draftCount;
        private int _pendingCount;
        private int _publishedCount;
        private DateTime _createdDate;
        private DateTime _startDate;
        private DateTime _endDate;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string CampaignName { get; set; } = string.Empty;

        public PublisherCampaignStatus Status { get; set; } = PublisherCampaignStatus.Completed;

        public int DestinationCount
        {
            get
            {
                return _destinationCount;
            }
            set
            {
                _destinationCount = value;
                OnPropertyChanged(nameof(DestinationCount));
            }
        }

        public int DraftCount
        {
            get
            {
                return _draftCount;
            }
            set
            {
                _draftCount = value;
                OnPropertyChanged(nameof(DraftCount));
            }
        }

        public int PendingCount
        {
            get
            {
                return _pendingCount;
            }
            set
            {
                _pendingCount = value;
                OnPropertyChanged(nameof(PendingCount));
            }
        }

        public int PublishedCount
        {
            get
            {
                return _publishedCount;
            }
            set
            {
                _publishedCount = value;
                OnPropertyChanged(nameof(PublishedCount));
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }


        #region Job Scheduling

        public List<ContentSelectGroup> ScheduledWeekday { get; set; } = new List<ContentSelectGroup>();

        public bool IsRotateDayChecked { get; set; }

        public List<TimeSpan> SpecificRunningTime { get; set; } = new List<TimeSpan>();

        public TimeRange TimeRange { get; set; } 
  
        public bool IsTakeRandomDestination { get; set; }

        public int TotalRandomDestination { get; set; }

        public int MinRandomDestinationPerAccount { get; set; }

        #endregion


        public void GenerateCampaign()
        {
            CampaignName = $"Campaign-{ConstantVariable.GetDateTime()}";
            CampaignId = Utilities.GetGuid();
            CreatedDate = DateTime.Today;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(7);
        }

        public void GenerateCloneCampaign(string name)
        {
            CampaignName = $"{name}-clone-{ConstantVariable.GetHourDateTime()}";
            CampaignId = Utilities.GetGuid();
            CreatedDate = DateTime.Today;
            IsSelected = false;
        }

        public bool ValidDateTime() => StartDate < EndDate;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}