using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public MediaType MediaType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public ActivityType ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string PkOwner { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int TakenAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string UsernameOwner { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Comment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string CommentId { get; set; }
    }
}
