#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.FacebookModels
{
    public interface IPostLikerCommentor
    {
        string CustomPostList { get; set; }

        string PageUrl { get; set; }

        string GroupUrl { get; set; }

        string Campaign { get; set; }


        int Count { get; set; }

        string Keyword { get; set; }
    }

    public class PostLikeCommentorModel : BindableBase, IPostLikerCommentor
    {
        private bool _isOwnWallChecked;

        [ProtoMember(1)]
        public bool IsOwnWallChecked
        {
            get => _isOwnWallChecked;
            set
            {
                if (value == _isOwnWallChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.OwnWall);
                SetProperty(ref _isOwnWallChecked, value);
            }
        }

        private bool _isNewsfeedChecked;

        [ProtoMember(2)]
        public bool IsNewsfeedChecked
        {
            get => _isNewsfeedChecked;
            set
            {
                if (value == _isNewsfeedChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.NewsFeed);
                SetProperty(ref _isNewsfeedChecked, value);
            }
        }

        private bool _isFriendTimeLineChecked;

        [ProtoMember(3)]
        public bool IsFriendTimeLineChecked
        {
            get => _isFriendTimeLineChecked;
            set
            {
                if (value == _isFriendTimeLineChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.FriendWall);
                SetProperty(ref _isFriendTimeLineChecked, value);
            }
        }

        private bool _isCustomPostListChecked;

        [ProtoMember(4)]
        public bool IsCustomPostListChecked
        {
            get => _isCustomPostListChecked;
            set
            {
                if (value == _isCustomPostListChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.CustomPostList);
                SetProperty(ref _isCustomPostListChecked, value);
            }
        }

        private bool _isCampaignChecked;

        [ProtoMember(5)]
        public bool IsCampaignChecked
        {
            get => _isCampaignChecked;
            set
            {
                if (value == _isCampaignChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.Campaign);
                SetProperty(ref _isCampaignChecked, value);
            }
        }

        private bool _isGroupChecked;

        [ProtoMember(6)]
        public bool IsGroupChecked
        {
            get => _isGroupChecked;
            set
            {
                if (value == _isGroupChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.Group);
                SetProperty(ref _isGroupChecked, value);
            }
        }

        private bool _isPageChecked;

        [ProtoMember(7)]
        public bool IsPageChecked
        {
            get => _isPageChecked;
            set
            {
                if (value == _isPageChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.Pages);
                SetProperty(ref _isPageChecked, value);
            }
        }


        private List<string> _listFriendProfileUrl = new List<string>();

        [ProtoMember(8)]
        public List<string> ListFriendProfileUrl
        {
            get => _listFriendProfileUrl;
            set
            {
                if (value == _listFriendProfileUrl)
                    return;
                SetProperty(ref _listFriendProfileUrl, value);
            }
        }

        private List<string> _listCustomPostList = new List<string>();

        [ProtoMember(9)]
        public List<string> ListCustomPostList
        {
            get => _listCustomPostList;
            set
            {
                if (value == _listCustomPostList)
                    return;
                SetProperty(ref _listCustomPostList, value);
            }
        }


        private List<string> _listFaceDominatorCampaign = new List<string>();

        [ProtoMember(10)]
        public List<string> ListFaceDominatorCampaign
        {
            get => _listFaceDominatorCampaign;
            set
            {
                if (value == _listFaceDominatorCampaign)
                    return;
                SetProperty(ref _listFaceDominatorCampaign, value);
            }
        }

        private List<string> _listGroupUrl = new List<string>();

        [ProtoMember(11)]
        public List<string> ListGroupUrl
        {
            get => _listGroupUrl;
            set
            {
                if (value == _listGroupUrl)
                    return;
                SetProperty(ref _listGroupUrl, value);
            }
        }


        private List<string> _listPageUrl = new List<string>();

        [ProtoMember(12)]
        public List<string> ListPageUrl
        {
            get => _listPageUrl;
            set
            {
                if (value == _listPageUrl)
                    return;
                SetProperty(ref _listPageUrl, value);
            }
        }

        private string _friendProfileUrl = string.Empty;

        [ProtoMember(13)]
        public string FriendProfileUrl
        {
            get => _friendProfileUrl;
            set
            {
                if (value == _friendProfileUrl)
                    return;
                SetProperty(ref _friendProfileUrl, value);
            }
        }

        private string _customPostList = string.Empty;

        [ProtoMember(14)]
        public string CustomPostList
        {
            get => _customPostList;
            set
            {
                if (value == _customPostList)
                    return;
                SetProperty(ref _customPostList, value);
            }
        }

        private string _pageUrl = string.Empty;

        [ProtoMember(15)]
        public string PageUrl
        {
            get => _pageUrl;
            set
            {
                if (value == _pageUrl)
                    return;
                SetProperty(ref _pageUrl, value);
            }
        }


        private string _groupUrl = string.Empty;

        [ProtoMember(16)]
        public string GroupUrl
        {
            get => _groupUrl;
            set
            {
                if (value == _groupUrl)
                    return;
                SetProperty(ref _groupUrl, value);
            }
        }

        private string _campaign = string.Empty;

        [ProtoMember(17)]
        public string Campaign
        {
            get => _campaign;
            set
            {
                if (value == _campaign)
                    return;
                SetProperty(ref _campaign, value);
            }
        }


        private List<string> _listAlbums = new List<string>();

        [ProtoMember(18)]
        public List<string> ListAlbums
        {
            get => _listAlbums;
            set
            {
                if (value == _listAlbums)
                    return;
                SetProperty(ref _listAlbums, value);
            }
        }

        private bool _isAlbumsChecked;

        [ProtoMember(19)]
        public bool IsAlbumsChecked
        {
            get => _isAlbumsChecked;
            set
            {
                if (value == _isAlbumsChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.Albums);
                SetProperty(ref _isAlbumsChecked, value);
            }
        }


        private int _count;

        [ProtoMember(20)]
        public int Count
        {
            get => _count;
            set
            {
                if (value == _count)
                    return;
                SetProperty(ref _count, value);
            }
        }


        private string _albums = string.Empty;

        [ProtoMember(21)]
        public string Albums
        {
            get => _albums;
            set
            {
                if (value == _albums)
                    return;
                SetProperty(ref _albums, value);
            }
        }


        private Dictionary<PostOptions, bool> _lstPostOptions = new Dictionary<PostOptions, bool>
        {
            {PostOptions.OwnWall, false},
            {PostOptions.NewsFeed, false},
            {PostOptions.Group, false},
            {PostOptions.Pages, false},
            {PostOptions.Campaign, false},
            {PostOptions.FriendWall, false},
            {PostOptions.CustomPostList, false},
            {PostOptions.Albums, false},
            {PostOptions.Keyword, false},
            {PostOptions.ProfileScraper, false}
        };

        [ProtoMember(22)]
        public Dictionary<PostOptions, bool> LstPostOptions
        {
            get => _lstPostOptions;
            set
            {
                if (value == _lstPostOptions)
                    return;
                SetProperty(ref _lstPostOptions, value);
            }
        }

        private bool _isKeywordChecked;

        [ProtoMember(23)]
        public bool IsKeywordChecked
        {
            get => _isKeywordChecked;
            set
            {
                if (value == _isKeywordChecked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.Keyword);
                SetProperty(ref _isKeywordChecked, value);
            }
        }


        private List<string> _listKeywords = new List<string>();

        [ProtoMember(24)]
        public List<string> ListKeywords
        {
            get => _listKeywords;
            set
            {
                if (value == _listKeywords)
                    return;
                SetProperty(ref _listKeywords, value);
            }
        }

        private string _keyword = string.Empty;

        [ProtoMember(25)]
        public string Keyword
        {
            get => _keyword;
            set
            {
                if (value == _keyword)
                    return;
                SetProperty(ref _keyword, value);
            }
        }

        private bool _isCampaignChked;

        [ProtoMember(26)]
        public bool IsCampaignChked
        {
            get => _isCampaignChked;
            set
            {
                if (value == _isCampaignChked)
                    return;
                ChangeCountAndAddToList(value, PostOptions.ProfileScraper);
                SetProperty(ref _isCampaignChked, value);
            }
        }


        private List<string> _listCampaign = new List<string>();

        [ProtoMember(27)]
        public List<string> ListCampaign
        {
            get => _listCampaign;
            set
            {
                if (value == _listCampaign)
                    return;
                SetProperty(ref _listCampaign, value);
            }
        }

        private string _nrlcampaign = string.Empty;

        [ProtoMember(28)]
        public string NrlCampaign
        {
            get => _nrlcampaign;
            set
            {
                if (value == _nrlcampaign)
                    return;
                SetProperty(ref _nrlcampaign, value);
            }
        }


        public void ChangeCountAndAddToList(bool value, PostOptions postOptions)
        {
            try
            {
                if (value)
                {
                    Count++;
                    LstPostOptions.Remove(postOptions);
                    LstPostOptions.Add(postOptions, true);
                }
                else
                {
                    Count--;
                    LstPostOptions.Remove(postOptions);
                    LstPostOptions.Add(postOptions, false);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}