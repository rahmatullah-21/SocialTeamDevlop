using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GdTables.Campaigns
{
    public class InteractedPosts
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public int InteractionDate { get; set; }

        [Column(Order = 3)]
        public MediaType MediaType { get; set; }

        [Column(Order = 4)]
        public ActivityType ActivityType { get; set; }

        [Column(Order = 5)]
        public string PkOwner { get; set; }

        [Column(Order = 6)]
        public int TakenAt { get; set; }

        [Column(Order = 7)]
        public string UsernameOwner { get; set; }


        [Column(Order = 8)]
        public string Username { get; set; }

        [Column(Order = 9)]
        public string Comment { get; set; }

        [Column(Order = 10)]
        public string OriginalMediaCode { get; set; }

        [Column(Order = 11)]
        public string OriginalMediaOwner { get; set; }
        [Column(Order = 12)]
        public string QueryType { get; set; }

        [Column(Order = 13)]
        public string QueryValue { get; set; }

        [Column(Order = 14)]
        public string CommentId { get; set; }

        [Column(Order = 15)]
        public string Permalink { get; set; }

        [Column(Order = 16)]
        public string Status { get; set; }
    }
}
