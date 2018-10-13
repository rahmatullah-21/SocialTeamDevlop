using SQLite;
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
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order =2)]
        public string ChannelName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order =3)]
        public string SubscribersCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order =4)]
        public string VideosCount { get; set; }
    }
}
