using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class ScrapePostModel : BindableBase
    {
        /// <summary>
        /// To specify facebook scrape post is checked
        /// </summary>
        private bool _isScrapeFacebookPost;
        [ProtoMember(1)]
        public bool IsScrapeFacebookPost
        {
            get
            {
                return _isScrapeFacebookPost;
            }
            set
            {
                if (_isScrapeFacebookPost == value)
                    return;
                _isScrapeFacebookPost = value;
                OnPropertyChanged(nameof(IsScrapeFacebookPost));
            }
        }
        /// <summary>
        /// To specify scraping details of facebook
        /// </summary>
        private string _addFdPostSource = string.Empty;
        [ProtoMember(2)]
        public string AddFdPostSource
        {
            get
            {
                return _addFdPostSource;
            }
            set
            {
                if (_addFdPostSource == value)
                    return;
                _addFdPostSource = value;
                OnPropertyChanged(nameof(AddFdPostSource));
            }
        }

        /// <summary>
        /// To specify pinterest scrape post is checked
        /// </summary>
        private bool _isScrapePinterestPost;
        [ProtoMember(3)]
        public bool IsScrapePinterestPost
        {
            get
            {
                return _isScrapePinterestPost;
            }
            set
            {
                if (_isScrapePinterestPost == value)
                    return;
                _isScrapePinterestPost = value;
                OnPropertyChanged(nameof(IsScrapePinterestPost));
            }
        }
        /// <summary>
        /// To specify scraping details of pinterest
        /// </summary>
        private string _addPdPostSource = string.Empty;
        [ProtoMember(4)]
        public string AddPdPostSource
        {
            get
            {
                return _addPdPostSource;
            }
            set
            {
                if (_addPdPostSource == value)
                    return;
                _addPdPostSource = value;
                OnPropertyChanged(nameof(AddPdPostSource));
            }
        }

        /// <summary>
        /// To specify twitter scrape post is checked
        /// </summary>
        private bool _isScrapeTwitterPost;
        [ProtoMember(5)]
        public bool IsScrapeTwitterPost
        {
            get
            {
                return _isScrapeTwitterPost;
            }
            set
            {
                if (_isScrapeTwitterPost == value)
                    return;
                _isScrapeTwitterPost = value;
                OnPropertyChanged(nameof(IsScrapeTwitterPost));
            }
        }

        /// <summary>
        /// To specify scraping details of Twitter
        /// </summary>
        private string _addTdPostSource = string.Empty;
        [ProtoMember(6)]
        public string AddTdPostSource
        {
            get
            {
                return _addTdPostSource;
            }
            set
            {
                if (_addTdPostSource == value)
                    return;
                _addTdPostSource = value;
                OnPropertyChanged(nameof(AddTdPostSource));
            }
        }


        private int _scrapeCount = 1;
        [ProtoMember(7)]
        public int ScrapeCount
        {
            get
            {
                return _scrapeCount;
            }
            set
            {
                if (_scrapeCount == value)
                    return;
                SetProperty(ref _scrapeCount, value);
            }
        }


        private int _startScrapeOnXminute = 30;
        [ProtoMember(8)]
        public int StartScrapeOnXminute
        {
            get
            {
                return _startScrapeOnXminute;
            }
            set
            {
                if (_startScrapeOnXminute == value)
                    return;
                SetProperty(ref _startScrapeOnXminute, value);
            }
        }
        private bool _isOriginalPostDetails = true;
        [ProtoMember(9)]
        public bool IsOriginalPostDetails
        {
            get { return _isOriginalPostDetails; }
            set
            {
                if (value == _isOriginalPostDetails)
                    return;
                if (value)
                    IsOwnPostDetails = false;
                SetProperty(ref _isOriginalPostDetails, value);
            }
        }
        private bool _isOwnPostDetails;
        [ProtoMember(10)]
        public bool IsOwnPostDetails
        {
            get { return _isOwnPostDetails; }
            set
            {
                if (value == _isOwnPostDetails)
                    return;
                if (value)
                    IsOriginalPostDetails = false;
                SetProperty(ref _isOwnPostDetails, value);
            }
        }

        private ObservableCollection<string> _lstScrapedPostDetails = new ObservableCollection<string>();
        [ProtoMember(11)]
        public ObservableCollection<string> LstScrapedPostDetails
        {
            get { return _lstScrapedPostDetails; }
            set { SetProperty(ref _lstScrapedPostDetails, value); }
        }
    }
}