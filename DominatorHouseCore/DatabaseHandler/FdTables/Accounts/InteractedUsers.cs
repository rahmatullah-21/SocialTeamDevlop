using DominatorHouseCore.DatabaseHandler.Common;
using System;

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class InteractedUsers : Entity
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
        public string ActivityType
        { get; set; }

        /// <summary>
        /// Contains FullName Of the Interacted User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string UserId
        { get; set; }

        /// <summary>
        /// Contains ProfileUrl Of the Interacted User
        /// </summary>       
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string UserProfileUrl
        { get; set; }

        /// <summary>
        /// Contains Detailed Info of the Interacted User in the Form of Jason String
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string DetailedUserInfo
        { get; set; }

        /// <summary>
        /// Contains TimeStamp when interacted with the User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string ScrapedProfileUrl { get; set; }
        
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public bool IsPublishedToWall { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string PostDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string PublishedUrl { get; set; }

      

    }
}
