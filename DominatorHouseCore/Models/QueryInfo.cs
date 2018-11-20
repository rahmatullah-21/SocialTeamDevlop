using System;
using System.ComponentModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{

    /// <summary>
    /// QueryInfo is used to save the user query value 
    /// </summary>
    [ProtoContract]
    public class QueryInfo : BindableBase, ICloneable
    {
        private string _id = Utilities.GetGuid(true);

        private string _queryValue;
        private bool _isCustomFilterSelected;
        private string _customFilters;
        private int _addedDateTime = DateTimeUtilities.GetEpochTime();
        private int _queryPriority;
        private string _queryTypeDisplayName;

        public static readonly QueryInfo NoQuery = new QueryInfo();
        private int _index;

        public int Index
        {
            get { return _index; }
            set
            {
                SetProperty(ref _index, value);
            }
        }
        /// <summary>
        /// Id is the unique id for the query, which contains guid without dashes
        /// </summary>
        [ProtoMember(1)]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != null && _id == value)
                    return;
                SetProperty(ref _id, value);
            }
        }

        private string _queryType;
        /// <summary>
        /// QueryType holds the index value of QueryParameters 
        /// </summary>
        [ProtoMember(2)]
        public string QueryType
        {
            get
            {
                return _queryType;
            }
            set
            {
                if (_queryType == value)
                    return;
                SetProperty(ref _queryType, value);                
            }
        }

        /// <summary>
        /// QueryValue holds the input value for selected query type
        /// </summary>
        [ProtoMember(3)]
        public string QueryValue
        {
            get
            {
                return _queryValue;
            }
            set
            {
                if (_queryValue != null && _queryValue == value)
                    return;
                SetProperty(ref _queryValue, value);                
            }
        }


        /// <summary>
        /// IsCustomFilterSelected holds whether the Query contains custom filters
        /// </summary>
        [ProtoMember(4)]
        public bool IsCustomFilterSelected
        {
            get
            {
                return _isCustomFilterSelected;
            }
            set
            {
                if (_isCustomFilterSelected == value)
                    return;
                SetProperty(ref _isCustomFilterSelected, value);
            }
        }


        [ProtoMember(5)]
        public string CustomFilters
        {
            get
            {
                return _customFilters;
            }
            set
            {
                if (_customFilters != null && _customFilters == value)
                    return;
                SetProperty(ref _customFilters, value);
            }
        }


        /// <summary>
        /// AddedDateTime holds when the query added datetime 
        /// </summary>
        [ProtoMember(6)]
        public int AddedDateTime
        {
            get
            {
                return _addedDateTime;
            }
            set
            {
                if (_addedDateTime == value)
                    return;
                SetProperty(ref _addedDateTime, value);
            }
        }


        /// <summary>
        /// QueryPriority defines the order which query are going to execute in business logic
        /// </summary>
        [ProtoMember(7)]
        public int QueryPriority
        {
            get
            {
                return _queryPriority;
            }
            set
            {
                if (_queryPriority == value)
                    return;
                SetProperty(ref _queryPriority, value);
            }
        }
        private bool _isQuerySlected;

        public bool IsQuerySelected
        {
            get
            {
                return _isQuerySlected;
            }
            set
            {
                if (_isQuerySlected == value)
                    return;
                SetProperty(ref _isQuerySlected, value);
            }
        }

        public string QueryTypeAsDisplayName()
        {
            var value = (UserQueryParameters)Enum.Parse(typeof(UserQueryParameters), QueryType);
            var descrKey = EnumUtility.GetDescriptionAttr(value);

            return descrKey.FromResourceDictionary();
        }

        public string QueryTypeAsDisplayName(Type queryParameterType)
        {
            try
            {
                var value = Enum.Parse(queryParameterType, QueryType);

                var descrKey = EnumUtility.GetDescriptionAttr((Enum)Enum.Parse(queryParameterType, value.ToString()));

                return descrKey.FromResourceDictionary();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return string.Empty;
        }


        [ProtoMember(8)]
        public string QueryTypeDisplayName
        {
            get
            {
                return _queryTypeDisplayName;
            }
            set
            {
                if (_queryTypeDisplayName != null && _queryTypeDisplayName == value)
                    return;

                var displayName = value;            // ConvertToDisplayName(value)
                SetProperty(ref _queryTypeDisplayName, displayName);
            }
        }


        /// <summary>
        /// 
        /// </summary>     
        /// <param name="template1">template1 is which holds both template's query after merge </param>
        /// <param name="template2">template2's query are merging to template1 </param>
        /// <returns></returns>
        public bool MergeTemplateQuery(string template1, string template2)
        {
            if (string.IsNullOrEmpty(template1) && string.IsNullOrEmpty(template2))
                return false;

            //var destinationTemplateModel = JsonConvert.DeserializeObject<TemplateModel>(template1);

            //var sourceTemplateModel = JsonConvert.DeserializeObject<TemplateModel>(template2);

            //JsonConvert.DeserializeObject<>() destinationTemplateModel.ActivitySettings


            return false;
        }

        public object Clone()
        {
            return (QueryInfo)MemberwiseClone();
        }
    }


    public enum UserQueryParameters
    {
        [Description("LangKeyHashtagPostS")]
        HashtagPost = 1,
        [Description("LangKeyHashtagUsers")]
        HashtagUsers = 2,
        [Description("LangKeyKeywords")]
        Keywords = 3,
        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers = 4,
        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings = 5,
        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 6,
        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowers = 7,
        [Description("LangKeyLocationUsers")]
        LocationUsers = 8,
        [Description("LangKeyLocationPosts")]
        LocationPosts = 9,
        [Description("LangKeyCustomUsersList")]
        CustomUsers = 10,
        [Description("LangKeySuggestedUsers")]
        SuggestedUsers = 11,
        [Description("LangKeyCustomPhotos")]
        CustomPhotos =12,
        [Description("LangKeyUsersWhoLikedPosts")]
        UsersWhoLikedPost = 13,
        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UsersWhoCommentedOnPost = 14,
        [Description("LangKeyFromSomeonesCircle")]
        FromSomeonesCircle=15,
        [Description("LangKeyFromCircleOfFollowers")]
        FromCircleOfFollowers=16,
        [Description("LangKeyFromCircleOfFollowings")]
        FromCircleOfFollowings=17,
        [Description("LangKeyBoardFollowers")]
        BoardFollowers = 18,
        [Description("LangKeyCustomBoard")]
        CustomBoard = 19,
        [Description("LangKeyCustomPin")]
        CustomPin = 20,
        [Description("LangKeyNewsfeed")]
        NewsFeedPins = 21,
        [Description("LangKeyCustomurl")]
        CustomUrl = 22
    }
}