using DominatorHouseCore.DatabaseHandler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
   
    public class InteractedAds : Entity
    {
        /// <summary>
        /// Contains QueryType For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string QueryType
        { get; set; }

        /// <summary>
        /// Contains QueryValue For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryValue { get; set; }


        /// <summary>
        /// Describes Activity 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType
        { get; set; }

        /// <summary>
        /// Contains FullName Of the Interacted User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string AdId
        { get; set; }

        /// <summary>
        /// Contains ProfileUrl Of the Interacted User
        /// </summary>       
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string PostId
        { get; set; }

        /// <summary>
        /// Contains Detailed Info of the Interacted User in the Form of Jason String
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string PageId
        { get; set; }

        /// <summary>
        /// Contains TimeStamp when interacted with the User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string AdCountry { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string IpDetails { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string ScrapedProfileUrl { get; set; }

    }
}
