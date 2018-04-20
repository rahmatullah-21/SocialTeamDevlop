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
    public class Friends
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
        public string IsDetailedUserInfoStored
        { get; set; }

        

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 3)]
        public string FullName
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 4)]
        [Unique]
        public string ProfileUrl
        { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 5)]
        [Unique]
        public string Location
        { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 6)]
        public string DetailedUserInfo { get; set; }

        
    }
}
