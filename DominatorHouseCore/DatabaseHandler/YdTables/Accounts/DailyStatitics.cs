using SQLite.CodeFirst;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
{
    public class DailyStatitics
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Date when statistics are entered in Unix Timestamp
        /// </summary>

        [Column(Order = 2)]
        public DateTime Date
        { get; set; }

        /// <summary>
        /// Followers count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 3)]
        public int Subscribers
        { get; set; }

        /// <summary>
        /// Followings count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 4)]
        public int Channels
        { get; set; }

        /// <summary>
        /// Tweets count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 5)]
        public int Views
        { get; set; }


    }
}
