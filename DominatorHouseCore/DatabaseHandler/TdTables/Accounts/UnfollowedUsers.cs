using System;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.TdTables.Accounts
{
    public class UnfollowedUsers
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string SinAccUsername { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string UnfollowSource
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string SourceType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int SourceFilter
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string Username
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int FollowBackStatus
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int FollowedBackDate
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public int InteractionTimeStamp
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public DateTime InteractionDate
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string ProcessType { get; set; }
    }
}
