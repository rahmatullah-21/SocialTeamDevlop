using DominatorHouseCore.Enums;
using SQLite.CodeFirst;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign
{
    public class InteractedPosts
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// EmailId of the Account from which Interaction has been done
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_ContentId", 1, IsUnique = true)]
        [Column(Order = 2)]
        public string AccountEmail { get; set; }

        /// <summary>
        /// Contains QueryType For Interaction
        /// </summary>
        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        /// <summary>
        /// Contains QueryValue For Interaction
        /// </summary>
        [Column(Order = 4)]
        public string QueryValue { get; set; }

        /// <summary>
        /// Describes Activity 
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_ContentId", 2, IsUnique = true)]
        [Column(Order = 5)]
        public string ActivityType { get; set; }

        /// <summary>
        /// Image or Video or Text
        /// </summary>

        [Column(Order = 6)]
        public MediaType MediaType { get; set; }

        /// <summary>
        /// Contains ContentId of the Post being interacted
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_ContentId", 3, IsUnique = true)]
        [Column(Order = 7)]
        public string ContentId { get; set; }


        /// <summary>
        /// Contains PostTitle of the Post being interacted
        /// </summary>
        [Column(Order = 8)]
        public string PostTitle { get; set; }

        /// <summary>
        /// Contains PostDiscussion of the Post being interacted
        /// </summary>
        [Column(Order = 9)]
        public string PostDescription { get; set; }

        /// <summary>
        /// Contains LikesCount On the Post being interacted
        /// </summary>
        [Column(Order = 10)]
        public string Likes { get; set; }

        /// <summary>
        /// Contains Comments Count On the Post being interacted
        /// </summary>
        [Column(Order = 11)]
        public string Comments { get; set; }

        /// <summary>
        /// TimeStamp when interacted with the Post
        /// </summary>
        [Column(Order = 12)]
        public int InteractionTimeStamp { get; set; }

    }
}
