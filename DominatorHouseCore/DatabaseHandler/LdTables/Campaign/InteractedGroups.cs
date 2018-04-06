using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.LdTables.Campaign
{
    public class InteractedGroups
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// EmailId of the Account from which Interaction has been done
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_GroupUrl", 1, IsUnique = true)]
        [Column(Order = 2)]
        public string AccountEmail { get; set; }

        /// <summary>
        /// Contains QueryType For Interaction
        /// </summary>
        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        /// <summary>
        /// Contains QueryValue For Interaction
        /// </summary>
        [Column(Order = 4)]
        public string QueryValue { get; set; }

        /// <summary>
        /// Describes Activity 
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_GroupUrl", 2, IsUnique = true)]
        [Column(Order = 5)]
        public string ActivityType { get; set; }

        /// <summary>
        /// Contains Name of the Group being interacted
        /// </summary>
        [Column(Order = 6)]
        public string GroupName { get; set; }

        /// <summary>
        /// Contains Id of the Group being interacted
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_GroupUrl", 3, IsUnique = true)]
        [Column(Order = 7)]
        public string GroupUrl { get; set; }
        
        /// <summary>
        /// Contains TotalMembers in the Group being interacted
        /// </summary>
        [Column(Order = 8)]
        public string TotalMembers { get; set; }

        /// <summary>
        /// Describes CommunityType of the Group being interacted
        /// </summary>
        [Column(Order = 9)]
        public string CommunityType { get; set; }

        /// <summary>
        /// Describes Membership Status For this Account in the Group being interacted
        /// </summary>
        [Column(Order = 10)]
        public string MembershipStatus { get; set; }
        
        /// <summary>
        /// TimeStamp when interacted with the Group
        /// </summary>
        [Column(Order = 11)]
        public int InteractionTimeStamp{ get; set; }

    }
}
