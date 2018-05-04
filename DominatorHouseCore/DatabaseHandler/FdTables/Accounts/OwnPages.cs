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
    public class OwnPages
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
        public string PageId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 3)]
        public string PageUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 4)]
        public string PageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 5)]
        public string PageType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 6)]
        public string ProfilePicUrl { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 7)]
        [Unique]
        public string CoverPicUrl { get; set; }

        [Column(Order = 8)]
        public DateTime InteractionDate { get; set; }
    }
}
