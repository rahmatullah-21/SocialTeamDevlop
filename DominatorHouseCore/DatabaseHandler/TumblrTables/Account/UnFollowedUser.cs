using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Account
{
    public class UnFollowedUser
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
        public string ActivityType
        { get; set; }


        /// <summary>
        /// Contains TimeStamp when interacted with the User
        /// </summary>
        [Column(Order = 5)]
        public int InteractionTimeStamp { get; set; }



        /// <summary>
        /// Contains whom we are unfollowing 
        /// </summary>
        [Column(Order = 6)]
        public string InteractedUsername { get; set; }
     
       
        /// <summary>
        /// Contains whom we are unfollowing 
        /// </summary>
        [Column(Order = 7)]
        public string UserName { get; set; }
       
        
        /// <summary>
        /// Contais the TemplateId
        /// </summary>
        [Column(Order = 8)]
        public string TemplateId { get; set; }

    }
}
