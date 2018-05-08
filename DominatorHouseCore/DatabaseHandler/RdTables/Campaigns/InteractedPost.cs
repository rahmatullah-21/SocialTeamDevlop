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
        public double RedditTimeStamp { get; set; }

        [Column(Order = 7)]
        public int InteractionDate { get; set; }

       
    }
}
