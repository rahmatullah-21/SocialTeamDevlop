using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
{
    public class UnjoinedGroups
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Contains Name Of the Group
        /// </summary>
        [Column(Order = 2)]
        public string GroupName
        { get; set; }

        /// <summary>
        /// Contains Url Of the Group
        /// </summary>
        [Column(Order = 3)]
        [Unique]
        public string GroupUrl
        { get; set; }

        /// <summary>
        /// Contains Profile Picture Url Of the Connection
        /// </summary>
        [Column(Order = 4)]
        public string TotalMembers { get; set; }

        /// <summary>
        /// Describe Connection Type If FirstDegree,SecondDegree Or ThirdPlusDegree
        /// </summary>
        [Column(Order = 5)]
        public string CommunityType
        { get; set; }

        /// <summary>
        /// Contains Connected TimeStamp with this Account
        /// </summary>
        [Column(Order = 6)]
        public string MembershipStatus { get; set; }

        /// <summary>
        /// Contains Interaction Time Stamp
        /// </summary>
        [Column(Order = 7)]
        public int UnjoinedTimeStamp { get; set; }
    }
}
