using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherCreateCampaignModel : BindableBase
    {
        public PublisherCreateCampaignModel()
        {
            CampaignId = Utilities.GetGuid(true);
        }
        [ProtoMember(1)]
        public string CampaignId { get; set; }

        private string _campaignName = $"Campaign {DateTimeUtilities.GetEpochTime()}";
        // To specify the campaign name
        [ProtoMember(2)]
        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                if (_campaignName == value)
                    return;
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private PublisherCampaignStatus _campaignStatus = PublisherCampaignStatus.Active;
        // To specify the campaign status
        [ProtoMember(3)]
        public PublisherCampaignStatus CampaignStatus
        {
            get
            {
                return _campaignStatus;
            }
            set
            {
                if (_campaignStatus == value)
                    return;
                _campaignStatus = value;
                OnPropertyChanged(nameof(CampaignStatus));
            }
        }


        [ProtoMember(5)]
        public JobConfigurationModel JobConfigurations { get; set; } = new JobConfigurationModel();
        [ProtoMember(6)]
        public OtherConfigurationModel OtherConfiguration { get; set; } = new OtherConfigurationModel();
        private PostDetailsModel _postDetailsModel { get; set; } = new PostDetailsModel();
        [ProtoMember(7)]
        public PostDetailsModel PostDetailsModel
        {
            get
            {
                return _postDetailsModel;
            }
            set
            {
                if (_postDetailsModel == value)
                    return;
                _postDetailsModel = value;
                OnPropertyChanged(nameof(PostDetailsModel));
            }
        }
        private ObservableCollection<string> _lstDestinationId = new ObservableCollection<string>();

        [ProtoMember(8)]
        public ObservableCollection<string> LstDestinationId
        {
            get
            {
                return _lstDestinationId;
            }
            set
            {
                if (_lstDestinationId == value)
                    return;
                _lstDestinationId = value;
                OnPropertyChanged(nameof(_lstDestinationId));
            }
        }
        private ScrapePostModel _scrapePostModel = new ScrapePostModel();
        [ProtoMember(9)]
        public ScrapePostModel ScrapePostModel
        {
            get
            {
                return _scrapePostModel;
            }
            set
            {
                if (_scrapePostModel == value)
                    return;
                _scrapePostModel = value;
                OnPropertyChanged(nameof(ScrapePostModel));
            }
        }
        private SharePostModel _sharePostModel = new SharePostModel();
        [ProtoMember(10)]
        public SharePostModel SharePostModel
        {
            get
            {
                return _sharePostModel;
            }
            set
            {
                if (_sharePostModel == value)
                    return;
                _sharePostModel = value;
                OnPropertyChanged(nameof(SharePostModel));
            }
        }
        [ProtoMember(11)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //private PublisherMediaViewerModel _publisherMediaViewerModel = new PublisherMediaViewerModel();
        //[ProtoMember(12)]
        //public PublisherMediaViewerModel PublisherMediaViewerModel
        //{
        //    get
        //    {
        //        return _publisherMediaViewerModel;
        //    }
        //    set
        //    {
        //        if (_publisherMediaViewerModel == value)
        //            return;
        //        _publisherMediaViewerModel = value;
        //        OnPropertyChanged(nameof(PublisherMediaViewerModel));
        //    }
        //}

        private ObservableCollection<PublisherRssFeedModel> _lstFeedUrl = new ObservableCollection<PublisherRssFeedModel>();
        [ProtoMember(13)]
        public ObservableCollection<PublisherRssFeedModel> LstFeedUrl
        {
            get
            {
                return _lstFeedUrl;
            }
            set
            {
                if (value == _lstFeedUrl)
                    return;
                SetProperty(ref _lstFeedUrl, value);
            }
        }

        private ObservableCollection<PublisherMonitorFolderModel> _lstFolderPath =
            new ObservableCollection<PublisherMonitorFolderModel>();
        [ProtoMember(14)]
        public ObservableCollection<PublisherMonitorFolderModel> LstFolderPath
        {
            get
            {
                return _lstFolderPath;
            }
            set
            {
                if (value == _lstFolderPath)
                    return;
                SetProperty(ref _lstFolderPath, value);
            }
        }


        private ObservableCollection<PostDetailsModel> _lstPostDetailsModel =
           new ObservableCollection<PostDetailsModel>();
        [ProtoMember(15)]
        public ObservableCollection<PostDetailsModel> LstPostDetailsModels
        {
            get
            {
                return _lstPostDetailsModel;
            }
            set
            {
                if (value == _lstPostDetailsModel)
                    return;
                SetProperty(ref _lstPostDetailsModel, value);
            }
        }
    }
}