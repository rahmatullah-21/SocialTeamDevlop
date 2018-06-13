using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class ScrapePostModel : BindableBase
    {

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
    }
}