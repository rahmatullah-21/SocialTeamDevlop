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
    public class InteractedGroups
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

       
        /// <summary>
        /// Contains QueryType For Interaction
        /// </summary>
        [Column(Order = 2)]
        public string QueryType
        { get; set; }

        /// <summary>
        /// Contains QueryValue For Interaction
        /// </summary>
        [Column(Order = 3)]
        public string QueryValue { get; set; }

        /// <summary>
        /// Describes Activity 
        /// </summary>        
        [Column(Order = 4)]
        public string ActivityType { get; set; }

        /// <summary>
        /// Contains Name of the Group being interacted
        /// </summary>
        [Column(Order = 5)]
        public string GroupName { get; set; }

        /// <summary>
        /// Contains Id of the Group being interacted
        /// </summary>
        [Column(Order = 6)]
        public string GroupUrl { get; set; }

        /// <summary>
        /// Contains TotalMembers in the Group being interacted
        /// </summary>
        [Column(Order = 7)]
        public string TotalMembers { get; set; }

        /// <summary>
        /// Describes CommunityType of the Group being interacted
        /// </summary>
        [Column(Order = 8)]
        public string GroupType { get; set; }
        /// <summary>
        /// Describes Membership Status For this Account in the Group being interacted
        /// </summary>
        [Column(Order = 9)]
        public string MembershipStatus { get; set; }

        /// <summary>
        /// TimeStamp when interacted with the Group
        /// </summary>
        [Column(Order = 10)]
        public int InteractionTimeStamp { get; set; }


        [Column(Order = 11)]
        public DateTime InteractionDateTime { get; set; }
    }
}
