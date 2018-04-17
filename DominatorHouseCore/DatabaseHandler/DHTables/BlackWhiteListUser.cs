using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.DHTables
{
    public class BlackWhiteListUser
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string UserName { get; set; }
        [Column(Order = 3)]
        public string Network { get; set; }
        [Column(Order = 4)]
        public string CategoryType { get; set; }

        [Column(Order = 5)]
        public DateTime AddedDateTime { get; set; }
    }
}
