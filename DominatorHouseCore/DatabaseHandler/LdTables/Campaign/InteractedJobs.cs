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

    public class InteractedJobs
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// EmailId of the Account from which Interaction has been done
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_JobPostUrl", 1, IsUnique = true)]
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
        [Index("Pk_AccountEmail_ActivityType_JobPostUrl", 2, IsUnique = true)]
        [Column(Order = 5)]
        public string ActivityType { get; set; }

        /// <summary>
        /// Contains Name of the Company For Job being interacted
        /// </summary>
        //[Column(Order = 6)]
        //public string CompanyName { get; set; }

        /// <summary>
        /// Contains Url of the JobPost being interacted
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_JobPostUrl", 3, IsUnique = true)]
        [Column(Order = 6)]
        public string JobPostUrl { get; set; }

        /// <summary>
        /// Contains title of the JobPost being interacted
        /// </summary>
        [Column(Order = 7)]
        public string JobTitle { get; set; }

        /// <summary>
        /// Contains DetailedInfo Regarding Interacted JobPost In Jason String Form
        /// </summary>
        [Column(Order = 8)]
        public string DetailedInfo { get; set; }

        /// <summary>
        /// TimeStamp when interacted with the JobPost
        /// </summary>
        [Column(Order = 9)]
        public int InteractionTimeStamp { get; set; }
    }
}
