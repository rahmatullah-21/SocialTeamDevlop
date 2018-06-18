using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherRssFeedModel : BindableBase
    {
        private string _feedUrl = string.Empty;
        [ProtoMember(1)]
        public string FeedUrl
        {
            get
            {
                return _feedUrl;
            }
            set
            {
                if (value == _feedUrl)
                    return;
                SetProperty(ref _feedUrl, value);
            }
        }
        private string _feedTemplate = string.Empty;
        [ProtoMember(2)]
        public string FeedTemplate
        {
            get
            {
                return _feedTemplate;
            }
            set
            {
                if (value == _feedTemplate)
                    return;
                SetProperty(ref _feedTemplate, value);
            }
        }
        private string _buttonContent = "Save to List";
        [ProtoIgnore]
        public string ButtonContent
        {
            get
            {
                return _buttonContent;
            }
            set
            {
                if (value == _buttonContent)
                    return;
                SetProperty(ref _buttonContent, value);
            }
        }
    }
}