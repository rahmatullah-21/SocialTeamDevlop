using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class InteractedPost
    {
        public int Id { get; set; }

        [Column(Order = 1)]
        public string RedditDescription { get; set; }

        [Column(Order = 2)]
        public int InteractionDate { get; set; }

        [Column(Order = 3)]
        public string Username { get; set; }

        [Column(Order = 4)]
        public string Query { get; set; }


        [Column(Order = 5)]
        public string QueryType
        { get; set; }

        [Column(Order = 6)]
        public double VideoDuration
        { get; set; }

        [Column(Order = 7)]
        public int ViewCount
        { get; set; }

        [Column(Order = 9)]
        public string RedditWebUrl { get; set; }

        [Column(Order = 10)]
        public ActivityType OperationType { get; set; }

        [Column(Order = 11)]
        public string UserId { get; set; }

        [Column(Order = 12)]
        public string BoardId { get; set; }

        [Column(Order = 23)]
        public string SinAccId { get; set; }

        [Column(Order = 24)]
        public string BriefInfo { get; set; }

        [Column(Order = 25)]
        public string CommentsCount { get; set; }

        [Column(Order = 26)]
        public string CommentUrl { get; set; }

        [Column(Order = 27)]
        public string PostCreationDate { get; set; }

        [Column(Order = 28)]
        public string Title { get; set; }

        [Column(Order = 29)]
        public string PointsCount { get; set; }

    }
}
