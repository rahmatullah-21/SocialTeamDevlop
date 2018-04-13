using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Campaigns
{
    public class InteractedBoards
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        //ID of the Board
        [Column(Order = 2)]
        public string BoardId { get; set; }

        [Column(Order = 3)]
        public string BoardName { get; set; }

        //Description of the Board
        [Column(Order = 4)]
        public string BoardDescription { get; set; }

        //Pin Count Of The Board
        [Column(Order = 5)]
        public int PinCount { get; set; }

        //Follower Count Of The Board
        [Column(Order = 6)]
        public int FollowerCount { get; set; }

        [Column(Order = 7)]
        public string Username { get; set; }

        [Column(Order = 8)]
        public string UserId { get; set; }

        [Column(Order = 9)]
        public string Query { get; set; }

        [Column(Order = 10)]
        public string QueryType
        { get; set; }

        [Column(Order = 11)]
        public int InteractionDate { get; set; }

        [Column(Order = 12)]
        public ActivityType OperationType { get; set; }

        [Column(Order = 13)]
        public string SinAccId { get; set; }

        [Column(Order = 14)]
        public string SinAccUsername { get; set; }


    }
}