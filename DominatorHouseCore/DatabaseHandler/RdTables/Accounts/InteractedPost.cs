using System;
using System.ComponentModel.DataAnnotations;
using DominatorHouseCore.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class InteractedPost
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }



        [Column(Order = 2)]
        public string RedditId { get; set; }

        [Column(Order = 3)]
        public string Query { get; set; }

        [Column(Order = 4)]
        public string QueryType { get; set; }

        [Column(Order = 5)]
        public string CommentsCount { get; set; }

        [Column(Order = 6)]
        public string ActivityType { get; set; }

        [Column(Order = 7)]
        public string Caption { get; set; }
        [Column(Order = 8)]
        public bool isCrosspostable { get; set; }
        [Column(Order = 9)]
        public bool isStickied { get; set; }

        //public object domainOverride { get; set; }
        //public object callToAction { get; set; }
        //public object[] eventsOnRender { get; set; }
        [Column(Order = 10)]
        public bool saved { get; set; }
        [Column(Order = 11)]
        public int numComments { get; set; }
        //public object upvoteRatio { get; set; }
        [Column(Order = 12)]
        public bool isPinned { get; set; }
        [Column(Order = 13)]
        public string author { get; set; }
        //public Media media { get; set; }
        [Column(Order = 14)]
        public int numCrossposts { get; set; }
        [Column(Order = 15)]
        public bool isSponsored { get; set; }
        [Column(Order = 16)]
        public string conflictid { get; set; }
        //public object contentCategories { get; set; }
        //public object source { get; set; }
        [Column(Order = 17)]
        public bool isLocked { get; set; }
        [Column(Order = 18)]
        public int score { get; set; }
        [Column(Order = 19)]
        public bool isArchived { get; set; }
        [Column(Order = 20)]
        public bool hidden { get; set; }
        [Column(Order = 21)]
        public string preview { get; set; }
        //public Thumbnail thumbnail { get; set; }
        //public Belongsto belongsTo { get; set; }
        [Column(Order = 22)]
        public bool isRoadblock { get; set; }
        //public object crosspostRootId { get; set; }
        //public object crosspostParentId { get; set; }
        [Column(Order = 23)]
        public bool sendReplies { get; set; }
        [Column(Order = 24)]
        public int goldCount { get; set; }
        [Column(Order = 25)]
        public bool isSpoiler { get; set; }
        [Column(Order = 26)]
        public bool isNSFW { get; set; }
        [Column(Order = 27)]
        public bool isMediaOnly { get; set; }
        [Column(Order = 28)]
        public string postId { get; set; }
        //public object suggestedSort { get; set; }
        [Column(Order = 29)]
        public bool isBlank { get; set; }
        [Column(Order = 30)]
        public int viewCount { get; set; }
        [Column(Order = 31)]
        public string permalink { get; set; }
        [Column(Order = 32)]
        public long created { get; set; }
        [Column(Order = 33)]
        public string title { get; set; }
        //public object[] events { get; set; }
        [Column(Order = 34)]
        public bool isOriginalContent { get; set; }
        //public object distinguishType { get; set; }
        [Column(Order = 35)]
        public int voteState { get; set; }

        [Column(Order = 36)]
        public int InteractionTimeStamp { get; set; }


        [Column(Order = 37)]
        public DateTime InteractionDateTime { get; set; }
        //[Column(Order = 30)]
        //public string RedditDescription { get; set; }

        //[Column(Order = 2)]
        //public int InteractionDate { get; set; }

        //[Column(Order = 3)]
        //public string Username { get; set; }

        //[Column(Order = 4)]
        //public string Query { get; set; }


        //[Column(Order = 5)]
        //public string QueryType
        //{ get; set; }

        //[Column(Order = 6)]
        //public double VideoDuration
        //{ get; set; }

        //[Column(Order = 7)]
        //public int ViewCount
        //{ get; set; }

        //[Column(Order = 9)]
        //public string RedditWebUrl { get; set; }

        //[Column(Order = 10)]
        //public ActivityType OperationType { get; set; }

        //[Column(Order = 11)]
        //public string UserId { get; set; }

        //[Column(Order = 12)]
        //public string BoardId { get; set; }

        //[Column(Order = 23)]
        //public string SinAccId { get; set; }

        //[Column(Order = 24)]
        //public string BriefInfo { get; set; }

        //[Column(Order = 25)]
        //public string CommentsCount { get; set; }

        //[Column(Order = 26)]
        //public string CommentUrl { get; set; }

        //[Column(Order = 27)]
        //public string PostCreationDate { get; set; }

        //[Column(Order = 28)]
        //public string Title { get; set; }

        //[Column(Order = 29)]
        //public string PointsCount { get; set; }

    }
}
