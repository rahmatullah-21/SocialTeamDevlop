using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
{
    public class InteractedPost
    {
        [Column(Order = 1)]
        public string RedditId { get; set; }

        [Column(Order = 2)]
        public string MediaString { get; set; }

        [Column(Order = 3)]
        public string RedditDescription { get; set; }

        [Column(Order = 4)]
        public string SubRedditUrl { get; set; }

        [Column(Order = 5)]
        public string header { get; set; }

        [Column(Order = 6)]
        public double BriefInfoqq { get; set; }

        [Column(Order = 7)]
        public int InteractionDate { get; set; }

        [Column(Order = 8)]
        public string CommentUrl { get; set; }

        [Column(Order = 9)]
        public string BriefInfo { get; set; }

        [Column(Order = 10)]
        public string PostCreationDate { get; set; }

        [Column(Order = 11)]
        public string Title { get; set; }

        [Column(Order = 12)]
        public string PointsCount { get; set; }

        [Column(Order = 13)]
        public string Query { get; set; }

        [Column(Order = 14)]
        public string QueryType { get; set; }

        [Column(Order = 15)]
        public string CommentsCount { get; set; }

        [Column(Order = 16)]
        public string ActivityType { get; set; }
    }
}
