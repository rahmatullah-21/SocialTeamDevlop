using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.DatabaseHandler.Common.Accounts;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class InteractedUsers : Entity, IInteractedUsers
    {
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int FollowedBack
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int FollowedBackDate
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTime
        {
            get { return ((IInteractedUsers)this).InteractionDate; }
            set { ((IInteractedUsers)this).InteractionDate = value; }
        }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ActivityType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string InteractedUsername
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public int Date
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string InteractedUserId
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int UpdatedTime
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int FollowersCount
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int FollowingsCount
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int PinsCount
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public int TriesCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string FullName
        { get; set; }
        
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public bool? HasAnonymousProfilePicture
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool IsVerified
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string ProfilePicUrl
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string Website { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string Bio { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string BoardDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string BoardUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string BoardName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public string Type { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public string DirectMessage
        { get; set; }

        int IInteractedUsers.InteractionDate { get; set; }
    }
}
