using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;
using static DominatorHouseCore.DatabaseHandler.GplusTables.Accounts.Communities;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class InteractedCommunities
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string Query { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        [Column(Order = 4)]
        public string CommunityId { get; set; }


        [Column(Order = 5)]
        public string CommunityUrl { get; set; }



        [Column(Order = 6)]
        public int Date
        { get; set; }

        [Column(Order = 7)]
        public string ActivityType
        { get; set; }



        [Column(Order = 8)]
        public JoinType joinType
        { get; set; }

        [Column(Order = 9)]
        public int MemberCounts
        { get; set; }





    }
}
