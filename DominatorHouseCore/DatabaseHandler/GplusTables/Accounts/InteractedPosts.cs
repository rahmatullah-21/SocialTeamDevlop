using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
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
        public ActivityType OperationType{ get; set; }

        [Column(Order = 5)]
        public string PostId { get; set; }

        [Column(Order = 6)]
        public int UploadedDate { get; set; }

        [Column(Order = 7)]
        public string PostOwnerId { get; set; }

        [Column(Order = 8)]
        public string PostOwnerName { get; set; }


        [Column(Order = 9)]
        public string Caption { get; set; }

        [Column(Order = 10)]
        public int LikeCount { get; set; }

        [Column(Order = 11)]
        public int CommentCount { get; set; }

        [Column(Order = 12)]
        public int ShareCount { get; set; }


    }
}
