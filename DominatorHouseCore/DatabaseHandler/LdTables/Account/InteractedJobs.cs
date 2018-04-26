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
  
    public class InteractedJobs
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
        [Index("Pk_ActivityType_JobPostUrl", 1, IsUnique = true)]
        [Column(Order = 4)]
        public string ActivityType { get; set; }

        /// <summary>
        /// Contains Url of the JobPost being interacted
        /// </summary>
        [Index("Pk_ActivityType_JobPostUrl", 2, IsUnique = true)]
        [Column(Order = 5)]
        public string JobPostUrl { get; set; }

        /// <summary>
        /// Contains title of the JobPost being interacted
        /// </summary>
        [Column(Order = 6)]
        public string JobTitle { get; set; }

        /// <summary>
        /// Contains DetailedInfo Regarding Interacted Job In Jason String Form
        /// </summary>
        [Column(Order = 7)]
        public string DetailedInfo { get; set; }

        /// <summary>
        /// TimeStamp when interacted with the JobPost
        /// </summary>
        [Column(Order = 8)]
        public DateTime InteractionDatetime { get; set; }
    }
}
