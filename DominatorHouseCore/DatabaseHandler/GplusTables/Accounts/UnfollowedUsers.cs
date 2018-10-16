using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class UnfollowedUsers
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order =1)]
        public int Id { get; set; } 

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string FilterArgument
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int FilterTypeSql
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int FollowedBack
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int FollowedBackDate
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionDate
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ActivityType
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string UserId
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string FullName
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string QueryType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int BlockedStatus
        { get; set; }


       

    }
}
