using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class DominatorAccountBaseModel
    {
        public DominatorAccountBaseModel()
        {
            AccountProxy.ProxyIp = string.Empty;
            AccountProxy.ProxyPort = string.Empty;
            AccountProxy.ProxyUsername = string.Empty;
            AccountProxy.ProxyPassword = string.Empty;
        }


     

        /// <summary>
        ///  To Identify Account is belongs to which network
        /// </summary>
        [ProtoMember(1)]
        public SocialNetworks AccountNetwork { get; set; } = SocialNetworks.Instagram;


        /// <summary>
        /// To define the account is belongs to which group
        /// </summary>
        [ProtoMember(2)]
        public ContentSelectGroup AccountGroup { get; set; } = new ContentSelectGroup();


        /// <summary>
        /// To define the social networks username of the accounts
        /// </summary>
        [ProtoMember(3)]
        public string UserName { get; set; } = string.Empty;


        /// <summary>
        /// To store the account password
        /// </summary>
        [ProtoMember(4)]
        public string Password { get; set; } = string.Empty;


        /// <summary>
        /// To define the social networks id of the account
        /// </summary>
        [ProtoMember(5)]
        public string UserId { get; set; } = string.Empty;


        /// <summary>
        /// To define the username of the account
        /// </summary>
        [ProtoMember(6)]
        public string UserFullName { get; set; } = string.Empty;


        /// <summary>
        /// To define the profile picture url of the account
        /// </summary>
        [ProtoMember(7)]
        public string ProfilePictureUrl { get; set; } = string.Empty;


        /// <summary>
        /// To define the Account Proxy
        /// </summary>       
        [ProtoMember(8)]
        public Proxy AccountProxy { get; set; } = new Proxy();


        /// <summary>
        /// To access the account with unique Id
        /// </summary>
        [ProtoMember(9)]
        public string AccountId { get; set; } = Utilities.GetGuid(true);


        /// <summary>
        /// To define the status of the account
        /// </summary>
        [ProtoMember(10)]
        public string Status { get; set; } = ConstantVariable.NotChecked;


    }
}
