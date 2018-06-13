using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class UnfollowedUsers
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
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
        public int OperationType
        { get; set; }

        [Column(Order = 8)]
        public string Username
        { get; set; }

        [Column(Order = 9)]
        public string UserId { get; set; }

        [Column(Order = 10)]
        public string FullName { get; set; }

        [Column(Order = 11)]
        public DateTime InteractionDateTime { get; set; }
        [Column(Order = 12)]
        public int InteractionTimeStamp { get; set; }
    }
}
