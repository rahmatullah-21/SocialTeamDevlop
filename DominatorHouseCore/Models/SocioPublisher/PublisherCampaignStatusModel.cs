using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{

    [Serializable]
    [ProtoContract]
    public class PublisherCampaignStatusModel : INotifyPropertyChanged
    {    
        public string CampaignId { get; set; }

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

        public string CampaignName { get; set; }

        public PublisherCampaignStatus Status { get; set; } = PublisherCampaignStatus.Completed;

        public int DestinationCount { get; set; }

        public int DraftCount { get; set; }

        public int PendingCount { get; set; }

        public int PublishedCount { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

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
            CampaignName = $"{name}-clone";
            CampaignId = Utilities.GetGuid();
            CreatedDate = DateTime.Today;
            IsSelected = false;
        }

        public bool ValidDateTime() 
            => StartDate < EndDate;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}