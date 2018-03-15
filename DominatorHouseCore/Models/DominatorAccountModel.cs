using System.Net;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using System;
using DominatorHouseCore.Request;
using Newtonsoft.Json;

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
            get
            {
                return _accountBaseModel;
            }
            set
            {
                if (_accountBaseModel != null && _accountBaseModel == value)
                    return;
                SetProperty(ref _accountBaseModel, value);
            }
        }

        #region Common Properties

        [ProtoMember(2)]
        public bool SelectedGroup { get; set; }

        // To display the account row position
        private int _rownumber;

        [ProtoMember(3)]
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
        [ProtoMember(4)]
        public bool IsAccountSelected { get; set; }

        private bool _bIsAccountManagerAccountSelected;

        [ProtoMember(5)]
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

        [ProtoMember(6)]
        public bool IsCretedFromNormalMode { get; set; }

        #endregion

        #region Job Scheduling

        // Stores  of the account in day wise
        [ProtoMember(7)]
        public JobActivityManager ActivityManager { get; set; } = new JobActivityManager();

        #endregion

        #region Http

        [ProtoIgnore]

        public HttpHelper HttpHelper { get; set; } = new HttpHelper();

        [ProtoIgnore]
        public CookieCollection Cookies { get; set; } = new CookieCollection();

        [ProtoIgnore]
        public bool IsloggedinWithPhone { get; set; }

        [ProtoIgnore]
        public string SessionId { get; set; } = string.Empty;

        [ProtoIgnore]
        public DeviceGenerator DeviceDetails { get; set; } = new DeviceGenerator();

        [ProtoIgnore]
        public bool IsUserLoggedIn { get; set; }

        [ProtoIgnore]
        public string UserAgentWeb { get; set; } =  string.Empty;

        [ProtoIgnore]
        public string UserAgentMobile { get; set; } = string.Empty;

        [ProtoIgnore]
        public int LastLogin { get; set; }

        #endregion

        #region Module Wise Details

        //It cont
        [ProtoMember(8)]
        public string ModulePrivateDetails { get; set; } 


        public string GetModulePrivateDetailsValue([CallerMemberName] string PropertyName = null)
        {
            try
            {
                return JObject.Parse(ModulePrivateDetails)[PropertyName].ToString();
            }
            catch (Exception e)
            {
                e.TraceLog();
                return string.Empty;
            }

        }


        public void UpdateModulePrivateDetailsValue(object model)
        {
            try
            {
                this.ModulePrivateDetails = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            }
            catch (Exception Ex)
            {
                Ex.TraceLog();
            }
        }


        public void SetModulePrivateDetailsValue(object value,[CallerMemberName]string PropertyName="")
        {
            try
            {
                JObject jObject = JObject.Parse(this.ModulePrivateDetails);
                jObject[PropertyName] = value.ToString();
                this.ModulePrivateDetails = jObject.ToString();
            }
            catch (Exception Ex)
            {
                Ex.TraceLog();
            }
        }




        #endregion

        #region Display column values
        // TODO: move those properties to DominatorAccountViewModel

        [ProtoIgnore]
        public int DisplayColumnValue1 { get; set; }

        [ProtoIgnore]
        public int DisplayColumnValue2 { get; set; }

        [ProtoIgnore]
        public int DisplayColumnValue3 { get; set; }

        [ProtoIgnore]
        public int DisplayColumnValue4 { get; set; } 

        #endregion

        #region Aliases of AccountBaseModel

        [ProtoIgnore]
        public string AccountId => AccountBaseModel?.AccountId;

        [ProtoIgnore]
        public string UserName => AccountBaseModel?.UserName;

        
        #endregion
    }
}
