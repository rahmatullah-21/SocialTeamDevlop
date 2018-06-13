using SQLite.CodeFirst;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
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
        public int TimeStamp
        { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Column(Order = 3)]
        public int Friends
        { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Column(Order = 4)]
        public int JoinedGroups
        { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Column(Order = 5)]
        public int OwnPages
        { get; set; }

        
    }
}
