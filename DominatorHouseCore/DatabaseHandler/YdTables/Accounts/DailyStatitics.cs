using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int Date
        { get; set; }

        /// <summary>
        /// Followers count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 3)]
        public int Followers
        { get; set; }

        /// <summary>
        /// Followings count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 4)]
        public int Followings
        { get; set; }

        /// <summary>
        /// Tweets count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 5)]
        public int Tweets
        { get; set; }

        /// <summary>
        /// Likes count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 6)]
        public int Likes { get; set; }

    }
}
