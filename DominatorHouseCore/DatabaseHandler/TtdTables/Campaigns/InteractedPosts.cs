using DominatorHouseCore.DatabaseHandler.Common;

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Campaigns
{
    public class InteractedPosts : Entity
    {
        /// <summary>
        /// Contains QueryType For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string QueryType
        { get; set; }

        /// <summary>
        /// Contains QueryValue For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryValue { get; set; }

        /// <summary>
        /// Describes Activity 
        /// </summary>     
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string AwemeId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string Description { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string CreateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string VideoUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string Duration { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string CommentCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string DiggCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string DownloadCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string PlayCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string ShareCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string Comment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string AccountUsername { get; set; }
    }
}
