using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
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
        public int Score
        { get; set; }

        /// <summary>
        /// Followings count of the DB owner when the statistics has got updated
        /// </summary>
       
        [Column(Order = 4)]
        public int Communities
        { get; set; }

        /// <summary>
        /// Tweets count of the DB owner when the statistics has got updated
        /// </summary>
        
        [Column(Order = 5)]
        public int PostKarma
        { get; set; }

        /// <summary>
        /// Likes count of the DB owner when the statistics has got updated
        /// </summary>

        [Column(Order = 6)]
        public int CommentKarma { get; set; }

    }
}
