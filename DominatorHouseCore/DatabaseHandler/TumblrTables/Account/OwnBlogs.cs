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
    public class OwnBlogs
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
        [Unique]
        public string BlogKey { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 3)]
        public string BlogUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 4)]
        public string BlogName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 5)]
        public int Postcount { get; set; }

        [Column(Order = 6)]
        public DateTime InteractionDate { get; set; }

    }
}
