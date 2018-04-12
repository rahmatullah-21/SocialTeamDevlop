using DominatorHouseCore.Enums;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Campaigns
{
  public  class InteractedPostsReport
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string Query { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        [Column(Order = 4)]
        public string ActivityType { get; set; }

        [Column(Order = 5)]
        public int InteractionDate { get; set; }

        [Column(Order = 6)]
        public MediaType MediaType { get; set; }

        [Column(Order = 7)]
        public string PostId { get; set; }

        [Column(Order = 8)]
        public int UploadedDate { get; set; }

        [Column(Order = 9)]
        public string PostOwnerId { get; set; }

        [Column(Order = 10)]
        public string PostOwnerName { get; set; }


        [Column(Order = 11)]
        public string Caption { get; set; }

        [Column(Order = 12)]
        public int LikeCount { get; set; }

        [Column(Order = 13)]
        public int CommentCount { get; set; }

        [Column(Order = 14)]
        public int ShareCount { get; set; }

        [Column(Order = 15)]
        public string PostUrl { get; set; }

        [Column(Order = 16)]
        public string Comment
        { get; set; }

        [Column(Order = 17)]
        public string InteractedUserId
        { get; set; }

        [Column(Order = 18)]
        public int IsLiked { get; set; }


    }
}
