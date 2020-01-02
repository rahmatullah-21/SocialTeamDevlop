using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.DHTables
{
    public class LoggerData
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public DateTime DateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public DateTime Network { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public DateTime AccountCampaign { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public DateTime ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public DateTime Message { get; set; }
    }
}
