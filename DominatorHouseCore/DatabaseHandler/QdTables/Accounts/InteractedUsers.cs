using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class InteractedUsers
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

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
        public int Date
        { get; set; }

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
        public string DirectMessage
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string InteractedUserId
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int Time
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public bool IsPrivate
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public bool IsBusiness
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public bool IsVerified
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool? IsProfilePicAvailable
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string ProfilePicUrl
        { get; set; }
    
    }
}
