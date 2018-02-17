using System;
using System.Net;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private DominatorAccountBaseModel _accountBaseModel;

        /// <summary>
        /// AccountBaseModel contains the base information of the account
        /// </summary>

        [ProtoMember(1)]
        public DominatorAccountBaseModel AccountBaseModel
        {
            get { return _accountBaseModel; }
            set
            {
                if (_accountBaseModel != null && _accountBaseModel == value)
                    return;
                SetProperty(ref _accountBaseModel, value);
            }
        }



        #region Common Properties

        public bool SelectedGroup { get; set; }

        // To display the account row position
        private int _rownumber;

        public int RowNo
        {
            get { return _rownumber; }
            set
            {
                if (_rownumber == value)
                    return;
                _rownumber = value;
                OnPropertyChanged(nameof(RowNo));
            }
        }

        // To define the account is selected or not 
        public bool IsAccountSelected { get; set; }

        private bool _bIsAccountManagerAccountSelected;

        public bool IsAccountManagerAccountSelected
        {
            get { return _bIsAccountManagerAccountSelected; }
            set
            {
                if (_bIsAccountManagerAccountSelected == value)
                    return;
                _bIsAccountManagerAccountSelected = value;
                OnPropertyChanged(nameof(IsAccountManagerAccountSelected));
            }
        }

        public bool IsCretedFromNormalMode { get; set; }

        #endregion

        #region Job Scheduling

        // Stores  of the account in day wise
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

        #region Module Wise Details

        //It cont
        public string ModulePrivateDetails { get; set; } = string.Empty;


        public object GetModulePrivateDetailsValue([CallerMemberName] string PropertyName = null)
        {
            try
            {
                return JObject.Parse(ModulePrivateDetails)[PropertyName];
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public void SetModulePrivateDetailsValue(object model)
        {
            try
            {
                this.ModulePrivateDetails = JsonConvert.SerializeObject(model);
            }
            catch (Exception Ex)
            {
            }
        }

        #endregion
    }


}
