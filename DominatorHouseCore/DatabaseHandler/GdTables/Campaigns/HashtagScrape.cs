using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GdTables.Campaigns
{
    public class HashtagScrape
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string AccountUsername { get; set; }

        [Column(Order = 3)]
        public ActivityType ActivityType { get; set; }

        [Column(Order = 4)]
        public string Keyword { get; set; }

        [Column(Order = 5)]
        public string HashtagName { get; set; }

        [Column(Order = 6)]
        public string HashtagId { get; set; }

        [Column(Order = 7)]
        public string MediaCount { get; set; }

        [Column(Order = 8)]
        public int Date { get; set; }
    }
}
