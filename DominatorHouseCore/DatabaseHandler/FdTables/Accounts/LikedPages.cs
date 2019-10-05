using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class LikedPages
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        //[Unique]
        public string PageId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string PageUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string PageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string PageType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string ProfilePicUrl { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        //[Unique]
        public string CoverPicUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public DateTime InteractionDate { get; set; }


    }
}
