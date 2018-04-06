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
    public class InteractedUsers
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// EmailId of the Account from which Interaction has been done
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_UserProfileUrl", 1, IsUnique = true)]
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

        [Index("Pk_AccountEmail_ActivityType_UserProfileUrl", 2, IsUnique = true)]
        /// <summary>
        /// Describes Activity 
        /// </summary>
        [Column(Order = 5)]
        public string ActivityType
        { get; set; }

        /// <summary>
        /// Contains FullName Of the Interacted User
        /// </summary>
        [Column(Order = 6)]
        public string UserFullName
        { get; set; }

        /// <summary>
        /// Contains ProfileUrl Of the Interacted User
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_UserProfileUrl", 3, IsUnique = true)]
        [Column(Order = 7)]
        public string UserProfileUrl
        { get; set; }

        /// <summary>
        /// Contains Detailed Info of the Interacted User in the Form of Jason String
        /// </summary>
        [Column(Order = 8)]
        public string DetailedUserInfo
        { get; set; }
        
        /// <summary>
        /// Contains TimeStamp when interacted with the User
        /// </summary>
        [Column(Order = 9)]
        public int InteractionTimeStamp { get; set; }
    }
}
