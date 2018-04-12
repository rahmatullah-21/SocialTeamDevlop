using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class PostCommets
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 2)]
        public string Keyword { get; set; }

        /// <summary>
        /// 
        /// </summary>        
        [Column(Order = 3)]
        public string ActivityType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 4)]
        public string CommentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 5)]
        public string CommentUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 6)]
        public string CommentorId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 7)]
        public string CommentText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 8)]
        public string CommetLikeCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 9)]
        public int CommentTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 10)]
        public int CommentPostId { get; set; }
    }
}
