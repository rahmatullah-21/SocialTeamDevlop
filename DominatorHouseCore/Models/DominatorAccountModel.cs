using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class DominatorAccountModel
    {
        public DominatorAccountBaseModel AccountBaseModel { get; set; }

        #region Common Properties

        public bool SelectedGroup { get; set; }

        // To display the account row position
        public int RowNo { get; set; }

        // To define the account is selected or not 
        public bool IsAccountSelected { get; set; }

        public bool IsAccountManagerAccountSelected { get; set; }

        #endregion

        #region Job Scheduling

        // To define activity of the account in day wise
        public JobActivityManager ActivityManager { get; set; } = new JobActivityManager();

        #endregion

        #region Http

        public HttpHelper HttpHelper { get; set; }

        public HttpHelper HttpHelperMobile { get; set; }

        public CookieCollection Cookies { get; set; }

        public bool IsloggedinWithPhone { get; set; }

        public string SessionId { get; set; }

        public DeviceGenerator DeviceDetails { get; set; } = new DeviceGenerator();

        public bool IsUserLoggedIn { get; set; }

        public string UserAgentWeb { get; set; }

        public string UserAgentMobile { get; set; }

        public int LastLogin { get; set; }

        #endregion

        #region Display Count

        public string Count { get; set; }

        #endregion

        #region Module Wise Details

        public string ModulePrivateDetails { get; set; }

        #endregion
    }
}
