using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GdTables.Campaigns
{
    public class FeedInfoes
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id
        { get; set; }

        [Column(Order = 2)]
        public string Caption
        { get; set; }

        [Column(Order = 3)]
        public int CommentCount
        { get; set; }

        [Column(Order = 4)]
        public bool CommentsDisabled
        { get; set; }

        [Column(Order = 5)]
        public string Preview
        { get; set; }


        [Column(Order = 6)]
        public int TakenAt
        { get; set; }


        [Column(Order = 7)]
        public double VideoDuration
        { get; set; }


        [Column(Order = 8)]
        public int ViewCount
        { get; set; }

        [Column(Order = 9)]
        public string MediaId { get; set; }

        [Column(Order = 10)]
        public MediaType MediaType { get; set; }

        [Column(Order = 11)]
        public string MediaCode { get; set; }

    }
}
