using SQLite.CodeFirst;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime Date
        { get; set; }

        /// <summary>
        /// Connections count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 3)]
        public int Followers
        { get; set; }

        /// <summary>
        /// LinkedinGroups count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 4)]
        public int Followings
        { get; set; }

        /// <summary>
        /// Posts count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 5)]
        public int PostsCount
        { get; set; }

        /// <summary>
        /// Likes count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 6)]
        public int ChannelsCount { get; set; }
    }
}
