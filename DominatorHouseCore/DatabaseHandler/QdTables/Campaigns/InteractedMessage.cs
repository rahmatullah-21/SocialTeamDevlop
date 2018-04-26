using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.QdTables.Campaigns
{
    public class InteractedMessage
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        [Column(Order = 4)]
        public string QueryValue { get; set; }

        [Column(Order = 5)]
        public string ActivityType { get; set; }

        [Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }


        [Column(Order = 9)]
        public string Username { get; set; }


        [Column(Order = 11)]
        public string Message { get; set; }



        [Column(Order = 17)]
        public int FollowBackStatus
        { get; set; }


        [Column(Order = 19)]
        public DateTime InteractionDate { get; set; }
    }
}
