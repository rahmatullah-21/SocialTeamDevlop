using DominatorHouseCore.Enums;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
{
    public class FeedInfo
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }
        
        /// <summary>
        /// Id of the Post
        /// </summary>
        [Column(Order = 3)]
        public string ContentId { get; set; }

        /// <summary>
        /// Image or Video or Text
        /// </summary>

        [Column(Order = 4)]
        public MediaType MediaType { get; set; }

        /// <summary>
        /// Title of the Post
        /// </summary>
        /// 

        [Column(Order = 5)]
        public string PostTitle { get; set; }

        /// <summary>
        /// Description of the Post
        /// </summary>
        /// 
        [Column(Order = 6)]
        public string PostDescription { get; set; }

        /// <summary>
        /// Like Count On The Post
        /// </summary>

        [Column(Order = 7)]
        public int LikeCount { get; set; }
        
        /// <summary>
        /// Comment Count On The Post
        /// </summary>

        [Column(Order = 8)]
        public int CommentCount { get; set; }
        
        /// <summary>
        /// Time when the Post had been Posted in TimeStamp
        /// </summary>
        /// 
        [Column(Order = 9)]
        public Int64 PostedTimeStamp { get; set; }

    }
}
