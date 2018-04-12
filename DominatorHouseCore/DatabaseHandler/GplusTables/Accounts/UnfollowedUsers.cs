using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class UnfollowedUsers
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order =1)]
        public int Id { get; set; } 

        [Column(Order = 2)]
        public string FilterArgument
        { get; set; }


        [Column(Order = 3)]
        public int FilterTypeSql
        { get; set; }


        [Column(Order = 4)]
        public int FollowedBack
        { get; set; }


        [Column(Order = 5)]
        public int FollowedBackDate
        { get; set; }


        [Column(Order = 6)]
        public int InteractionDate
        { get; set; }


        [Column(Order = 7)]
        public string ActivityType
        { get; set; }


        [Column(Order = 8)]
        public string UserId
        { get; set; }

        [Column(Order = 9)]
        public string FullName
        { get; set; }

        [Column(Order = 10)]
        public string QueryType
        { get; set; }

        [Column(Order = 11)]
        public int BlockedStatus
        { get; set; }


       

    }
}
