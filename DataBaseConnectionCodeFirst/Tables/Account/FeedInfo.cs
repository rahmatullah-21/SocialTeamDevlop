using SQLite.CodeFirst;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseConnection.CommonDatabaseConnection.Tables.Account
{
    public class FeedInfoes
    {
        [Key]
        [Column(Order = 1)]
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
        public int CommentsDisabled
        { get; set; }

        [Column(Order = 5)]
        public int Preview
        { get; set; }


        [Column(Order = 6)]
        public int TakenAt
        { get; set; }


        [Column(Order = 7)]
        public int VideoDuration
        { get; set; }


        [Column(Order = 8)]
        public int ViewCount
        { get; set; }

    }
}
