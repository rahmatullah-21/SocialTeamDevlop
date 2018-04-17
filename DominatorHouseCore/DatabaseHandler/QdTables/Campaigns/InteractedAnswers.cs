using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.QdTables.Campaigns
{
    public class InteractedAnswers
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string QueryType { get; set; }

        [Column(Order = 3)]
        public string QueryValue { get; set; }

        [Column(Order = 4)]
        public string ActivityType { get; set; }

        [Column(Order = 5)]
        public DateTime InteractionDateTime { get; set; }
        [Column(Order = 6)]
        public string AnswersUrl { get; set; }
        [Column(Order = 7)]
        public string AnsweredUserName { get; set; }

    }
}
