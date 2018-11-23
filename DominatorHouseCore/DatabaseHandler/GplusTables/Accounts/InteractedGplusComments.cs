using SQLite;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class InteractedGplusComments
    {

        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        /// <summary>
        /// Contains QueryValue For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        /// <summary>
        /// 
        /// </summary>        
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string CommentText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string CommenterName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string CommenterUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string PostUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string PostId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public long CommentTimeStamp { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string Mentions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int CommentLikeCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int HasLikedByUser { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string PostOwnerName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string PostOwnerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public long InteractionTimeStamp { get; set; }

    }
}