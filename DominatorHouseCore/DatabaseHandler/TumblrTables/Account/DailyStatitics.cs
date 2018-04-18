using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Account
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
        public int TimeStamp
        { get; set; }

        /// <summary>
        /// Connections count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 3)]
        public int Connections
        { get; set; }

        /// <summary>
        /// LinkedinGroups count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 4)]
        public int LinkedinGroups
        { get; set; }

        /// <summary>
        /// Posts count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 5)]
        public int Posts
        { get; set; }

        /// <summary>
        /// Likes count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 6)]
        public int Likes { get; set; }

        /// <summary>
        /// Comments count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 7)]
        public int Comments { get; set; }

    }
}
