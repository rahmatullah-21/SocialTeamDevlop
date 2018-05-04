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
    public class OwnGroups
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
        public string GroupId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 3)]
        public string GroupUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 4)]
        public string GroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 5)]
        public string GroupType { get; set; }

        [Column(Order = 6)]
        public DateTime InteractionDate { get; set; }

    }
}
