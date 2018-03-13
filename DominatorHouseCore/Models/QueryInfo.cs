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

        public string QueryTypeAsDisplayName()
        {
            var value = (UserQueryParameters)Enum.Parse(typeof(UserQueryParameters), QueryType);
            var descrKey = EnumUtility.GetDescriptionAttr(value);

            return descrKey.FromResourceDictionary();
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
            return this.MemberwiseClone();
        }
    }


    public enum UserQueryParameters
    {
        [Description("langHashTagPost")]
        HashtagPost = 1,
        [Description("langHashtagusers")]
        HashtagUsers = 2,
        [Description("langKeywords")]
        Keywords = 3,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers = 4,
        [Description("langSomeonesFollowings")]
        SomeonesFollowings = 5,
        [Description("langFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 6,
        [Description("langFollowersOfSomeonesFollowers")]
        FollowersOfFollowers = 7,
        [Description("langLocationUsers")]
        LocationUsers = 8,
        [Description("langLocationPosts")]
        LocationPosts = 9,
        [Description("langCustomUser")]
        CustomUsers = 10,
        [Description("langSuggestedUsers")]
        SuggestedUsers = 11,
        [Description("langCustomPhotos")]
        CustomPhotos =12,
        [Description("langUsersWhoLiked")]
        UsersWhoLikedPost = 13,
        [Description("langUsersWhoCommented")]
        UsersWhoCommentedOnPost = 14,
        [Description("langFromSomeonesCircle")]
        FromSomeonesCircle=15,
        [Description("langFromCircleOfFollowers")]
        FromCircleOfFollowers=16,
        [Description("langFromCircleOfFollowings")]
        FromCircleOfFollowings=17
    }
}