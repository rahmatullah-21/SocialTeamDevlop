using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.TdTables.Accounts
{
    public class DailyGrowth
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// UserName of Account
        /// </summary>


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string UserId
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string UserName
        { get; set; }

        /// <summary>
        /// Date when statistics are entered in Unix Timestamp
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public System.DateTime Date
        { get; set; }
       
        /// <summary>
        /// Followers count of the DB owner when the statistics has got updated
        /// </summary>
        
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Followers
        { get; set; }

        /// <summary>
        /// Followings count of the DB owner when the statistics has got updated
        /// </summary>
       
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int Followings
        { get; set; }

        /// <summary>
        /// Tweets count of the DB owner when the statistics has got updated
        /// </summary>
        
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int Tweets
        { get; set; }

        /// <summary>
        /// Likes count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int Likes { get; set; }

    }
}
