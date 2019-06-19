using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace DominatorHouseCore.Models.FacebookModels
{
    public interface IInviterDetails
    {
    }

    public class InviterDetails : BindableBase, IInviterDetails
    {
        private bool _isRandomPost = true;

        [ProtoMember(1)]
        public bool IsRandomPosts
        {
            get { return _isRandomPost; }
            set
            {
                if (value == _isRandomPost)
                    return;
                SetProperty(ref _isRandomPost, value);
            }
        }


        private bool _isSpecificPosts;

        [ProtoMember(2)]
        public bool IsSpecificPosts
        {
            get { return _isSpecificPosts; }
            set
            {
                if (value == _isSpecificPosts)
                    return;
                SetProperty(ref _isSpecificPosts, value);
            }
        }


        private List<string> _listPostUrl = new List<string>();

        [ProtoMember(3)]
        public List<string> ListPostUrl
        {
            get { return _listPostUrl; }
            set
            {
                if (value == _listPostUrl)
                    return;
                SetProperty(ref _listPostUrl, value);
            }
        }


        private List<string> _listEventUrl = new List<string>();

        [ProtoMember(4)]
        public List<string> ListEventUrl
        {
            get { return _listEventUrl; }
            set
            {
                if (value == _listEventUrl)
                    return;
                SetProperty(ref _listEventUrl, value);
            }
        }


        private bool _isProfileUrl;

        [ProtoMember(5)]
        public bool IsProfileUrl
        {
            get { return _isProfileUrl; }
            set
            {
                if (value == _isProfileUrl)
                    return;
                SetProperty(ref _isProfileUrl, value);
            }
        }

        private bool _isPostUrl=true;

        [ProtoMember(6)]
        public bool IsPostUrl
        {
            get { return _isPostUrl; }
            set
            {
                if (value == _isPostUrl)
                    return;
                SetProperty(ref _isPostUrl, value);
            }
        }

        private string _eventUrls = string.Empty;

        [ProtoMember(7)]
        public string EventUrls
        {
            get { return _eventUrls; }
            set
            {
                if (value == _eventUrls)
                    return;
                SetProperty(ref _eventUrls, value);
            }
        }

        private List<string> _listEventUrls = new List<string>();

        [ProtoMember(8)]
        public List<string> ListWatchPartyUrls
        {
            get { return _listEventUrls; }
            set
            {
                if (value == _listEventUrls)
                    return;
                SetProperty(ref _listEventUrls, value);
            }
        }

        private string _watchPartyUrls = string.Empty;

        [ProtoMember(9)]
        public string WatchPartyUrls
        {
            get { return _watchPartyUrls; }
            set
            {
                if (value == _watchPartyUrls)
                    return;
                SetProperty(ref _watchPartyUrls, value);
            }
        }

        private bool _isGroupMember;

        [ProtoMember(10)]
        public bool IsGroupMember
        {
            get { return _isGroupMember; }
            set
            {
                if (value == _isGroupMember)
                    return;
                SetProperty(ref _isGroupMember, value);
            }
        }



        private string _postUrlText = string.Empty;

        [ProtoMember(11)]
        public string PostUrlText
        {
            get { return _postUrlText; }
            set
            {
                if (value == _postUrlText)
                    return;
                SetProperty(ref _postUrlText, value);
            }
        }
    }
}
