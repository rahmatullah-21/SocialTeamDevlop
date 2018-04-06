using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class Communities
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public int InteractionDate { get; set; }

        [Column(Order = 3)]
        public string CommunityId { get; set; }

        [Column(Order = 4)]
        public string CommunityUrl { get; set; }




        [Column(Order = 5)]
        public JoinType joinType
        { get; set; }

        [Column(Order = 6)]
        public int MemberCounts
        { get; set; }




        public enum JoinType : int
        {
            Joined, NotJoined, UnJoined
        }
    }
}
