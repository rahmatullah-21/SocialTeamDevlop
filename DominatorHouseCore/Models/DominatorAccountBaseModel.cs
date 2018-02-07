using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class DominatorAccountBaseModel
    {

        /// <summary>
        ///  To Identify Account is belongs to which network
        /// </summary>
        public SocialNetworks AccountNetwork { get; set; } = SocialNetworks.Instagram;

        /// <summary>
        /// To define the account is belongs to which group
        /// </summary>
        public ContentSelectGroup AccountGroup { get; set; }

        /// <summary>
        /// To define the social networks username of the accounts
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// To store the account password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// To define the social networks id of the account
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// To define the username of the account
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// To define the profile picture url of the account
        /// </summary>
        public string ProfilePictureUrl { get; set; }

        /// <summary>
        /// To define the Account Proxy
        /// </summary>       
        public Proxy AccountProxy { get; set; }

        /// <summary>
        /// To access the account with unique Id
        /// </summary>
        public string AccountId { get; set; } = Utilities.GetGuid(true);

        /// <summary>
        /// To define the status of the account
        /// </summary>
        public string Status { get; set; }

    }
}
