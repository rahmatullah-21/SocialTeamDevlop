using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
{
    public class OwnChannels
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Column(Order =2)]
        public string ChannelName { get; set; }

        [Column(Order =3)]
        public string SubscribersCount { get; set; }

        [Column(Order =4)]
        public string VideosCount { get; set; }
    }
}
