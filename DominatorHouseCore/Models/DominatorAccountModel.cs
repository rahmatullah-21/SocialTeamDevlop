using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    /// <summary>
    /// Reprents each account loaded from %localappdata%/.../AccountDetails.bin
    /// Contains ActivityManager with Jobs/Activites for this account.
    /// </summary>
    [ProtoContract]
    public sealed class DominatorAccountModel : BindableBase
    {
        /// <summary>
        /// AccountBaseModel contains the base information of the account
        /// </summary>
        [ProtoMember(1)]      
        public DominatorAccountBaseModel AccountBaseModel { get; set; }

        #region Common Properties

        public bool SelectedGroup { get; set; }

        // To display the account row position
        public int RowNo { get; set; }

        // To define the account is selected or not 
        public bool IsAccountSelected { get; set; }

        public bool IsAccountManagerAccountSelected { get; set; }

        public bool IsCretedFromNormalMode { get; set; }

        #endregion

        #region Job Scheduling

        // To define activity of the account in day wise
        public JobActivityManager ActivityManager { get; set; } = new JobActivityManager();

        #endregion

        #region Http

        public HttpHelper HttpHelper { get; set; } = new HttpHelper();

        public CookieCollection Cookies { get; set; } = new CookieCollection();

        public bool IsloggedinWithPhone { get; set; }

        public string SessionId { get; set; } = string.Empty;

        public DeviceGenerator DeviceDetails { get; set; } = new DeviceGenerator();

        public bool IsUserLoggedIn { get; set; }

        public string UserAgentWeb { get; set; } = string.Empty;

        public string UserAgentMobile { get; set; } = string.Empty;

        public int LastLogin { get; set; }

        #endregion

        #region Display Count

        public string Count { get; set; } = "0";

        #endregion

        #region Module Wise Details

        public string ModulePrivateDetails { get; set; } = string.Empty;

        #endregion        
    }
}
