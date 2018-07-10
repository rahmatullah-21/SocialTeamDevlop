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

        public int DestinationCount { get; set; }

        public int DraftCount { get; set; }

        public int PendingCount { get; set; }

        public int PublishedCount { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


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