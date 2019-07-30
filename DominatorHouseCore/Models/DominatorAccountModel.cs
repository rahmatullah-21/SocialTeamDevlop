using DominatorHouseCore.EmailService;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

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

        private bool _IsAccountManagerAccountSelected;

        [ProtoIgnore]
        public bool IsAccountManagerAccountSelected
        {
            get { return _IsAccountManagerAccountSelected; }
            set
            {
                if (_IsAccountManagerAccountSelected == value)
                    return;
                _IsAccountManagerAccountSelected = value;
                OnPropertyChanged(nameof(IsAccountManagerAccountSelected));
            }
        }

        [Obsolete("Dont use this property, instead use DominatorHouseCore.Utility.ModuleConfiguration.IsTemplateMadeByCampaignMode property", true)]
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
        public bool IsUserLoggedIn { get; set; }

        [ProtoMember(9)]
        public string UserAgentWeb { get; set; } = string.Empty;

        [ProtoMember(10)]
        public string UserAgentMobile { get; set; } = string.Empty;

        [ProtoMember(11)]
        public bool UseMobileRequestOnly { get; set; }

        [ProtoIgnore]
        public bool IsloggedinWithPhone { get; set; }

        [ProtoIgnore]
        public string SessionId { get; set; } = string.Empty;

        [ProtoIgnore]
        public DeviceGenerator DeviceDetails { get; set; } = new DeviceGenerator();

        [ProtoIgnore]
        public int LastLogin { get; set; }

        [ProtoMember(21)]
        public int LastUpdateTime { get; set; }

        #endregion

        #region Module Wise Details

        //It cont
        [ProtoMember(12)]
        public string ModulePrivateDetails { get; set; }


        public string GetModulePrivateDetailsValue([CallerMemberName] string PropertyName = null)
        {
            try
            {
                return ModulePrivateDetails == null
                    ? null
                    : JObject.Parse(ModulePrivateDetails)[PropertyName].ToString();
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
                ModulePrivateDetails = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            }
            catch (Exception Ex)
            {
                Ex.TraceLog();
            }
        }


        public void SetModulePrivateDetailsValue(object value, [CallerMemberName] string PropertyName = "")
        {
            try
            {
                JObject jObject = JObject.Parse(ModulePrivateDetails);
                jObject[PropertyName] = value.ToString();
                ModulePrivateDetails = jObject.ToString();
            }
            catch (Exception Ex)
            {
                Ex.TraceLog();
            }
        }

        #endregion

        #region Display column values

        // TODO: change the way we store and display module-specific data

        [ProtoMember(16)]
        public int? DisplayColumnValue1
        {
            get { return _displayColumnValue1; }
            set { SetProperty(ref _displayColumnValue1, value); }
        }

        [ProtoMember(17)]
        public int? DisplayColumnValue2
        {
            get { return _displayColumnValue2; }
            set { SetProperty(ref _displayColumnValue2, value); }
        }

        [ProtoMember(18)]
        public int? DisplayColumnValue3
        {
            get { return _displayColumnValue3; }
            set { SetProperty(ref _displayColumnValue3, value); }
        }

        [ProtoMember(19)]
        public int? DisplayColumnValue4
        {
            get { return _displayColumnValue4; }
            set { SetProperty(ref _displayColumnValue4, value); }
        }

        [ProtoMember(20)]
        public int? DisplayColumnValue5
        {
            get { return _displayColumnValue5; }
            set { SetProperty(ref _displayColumnValue5, value); }
        }

        [ProtoIgnore]
        public int? DisplayColumnValue6
        {
            get { return _displayColumnValue6; }
            set { SetProperty(ref _displayColumnValue6, value); }
        }

        [ProtoIgnore]
        public int? DisplayColumnValue7
        {
            get { return _displayColumnValue7; }
            set { SetProperty(ref _displayColumnValue7, value); }
        }
        [ProtoIgnore]
        public int? DisplayColumnValue8
        {
            get { return _displayColumnValue8; }
            set { SetProperty(ref _displayColumnValue8, value); }
        }
        [ProtoIgnore]
        public int? DisplayColumnValue9
        {
            get { return _displayColumnValue9; }
            set { SetProperty(ref _displayColumnValue9, value); }
        }
        [ProtoIgnore]
        public int? DisplayColumnValue10
        {
            get { return _displayColumnValue10; }
            set { SetProperty(ref _displayColumnValue10, value); }
        }

        #endregion

        #region Aliases of AccountBaseModel

        [ProtoMember(15)]
        public string AccountId { get; set; }

        [ProtoIgnore]
        public string UserName => AccountBaseModel?.UserName;

        #endregion


        private HashSet<CookieHelper> _cookieHelperList = new HashSet<CookieHelper>();
        [ProtoMember(13)]
        public HashSet<CookieHelper> CookieHelperList
        {
            get { return _cookieHelperList; }
            set
            {
                if (_cookieHelperList != null && _cookieHelperList == value)
                    return;
                SetProperty(ref _cookieHelperList, value);
            }
        }

        private int? _displayColumnValue1;
        private int? _displayColumnValue2;
        private int? _displayColumnValue3;
        private int? _displayColumnValue4;
        private int? _displayColumnValue5;
        private int? _displayColumnValue6;
        private int? _displayColumnValue7;
        private int? _displayColumnValue8;
        private int? _displayColumnValue9;
        private int? _displayColumnValue10;

        [ProtoIgnore]
        public CookieCollection Cookies
        {
            get
            {
                var cookieCollection = new CookieCollection();

                if (_cookieHelperList != null)
                {
                    foreach (var cookieHelper in _cookieHelperList)
                        cookieCollection.Add(new Cookie()
                        {
                            Domain = cookieHelper.Domain,
                            Name = cookieHelper.Name,
                            Value = cookieHelper.Value
                        });
                }

                return cookieCollection;
            }
            set
            {
                _cookieHelperList = value?.Cast<Cookie>().Select(cookie => new CookieHelper
                {
                    Domain = cookie.Domain,
                    Name = cookie.Name,
                    Value = cookie.Value
                }).ToHashSet();
            }
        }

        [ProtoMember(14)]
        public Dictionary<string, string> ExtraParameters { get; set; }
            = new Dictionary<string, string>();


        public CancellationTokenSource CancellationSource = new CancellationTokenSource();

        public CancellationToken Token
        {
            get
            {
                return CancellationSource.Token;
            }
        }

        public void NotifyCancelled()
        {
            CancellationSource.Cancel();
        }

        private string _varificationCode = string.Empty;

        public string VarificationCode
        {
            get
            {
                return _varificationCode;
            }
            set
            {
                if (_varificationCode == value)
                    return;
                SetProperty(ref _varificationCode, value);
            }
        }

        private MailCredentials _mailCredentials = new MailCredentials();
        [ProtoMember(22)]
        public MailCredentials MailCredentials
        {
            get
            {
                return _mailCredentials;
            }
            set
            {
                if (_mailCredentials == value)
                    return;
                SetProperty(ref _mailCredentials, value);
            }
        }
        private bool _isAutoVerifyByEmail;
        [ProtoMember(23)]
        public bool IsAutoVerifyByEmail
        {
            get
            {
                return _isAutoVerifyByEmail;
            }
            set
            {

                if (_isAutoVerifyByEmail == value)
                    return;
                if (value)
                    IsManualVerify = false;
                SetProperty(ref _isAutoVerifyByEmail, value);
            }
        }
        private bool _isUseSSL;

        [ProtoMember(24)]
        public bool IsUseSSL
        {
            get { return _isUseSSL; }
            set { SetProperty(ref _isUseSSL, value); }
        }

        /// <summary>
        /// Using ActivityType:Querytype:Queryvalue as a key
        /// </summary>
        [ProtoMember(25)]
        public Dictionary<string, string> PaginationId { get; set; }
            = new Dictionary<string, string>();

        private string _newPassword = string.Empty;

        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                SetProperty(ref _newPassword, value);
            }
        }



        private string _resetPasswordLink = string.Empty;

        public string ResetPasswordLink
        {
            get
            {
                return _resetPasswordLink;
            }
            set
            {
                SetProperty(ref _resetPasswordLink, value);
            }
        }

        private bool _isVerificationCodeSent;
        public bool IsVerificationCodeSent
        {
            get
            {
                return _isVerificationCodeSent;
            }
            set
            {
                if (_isVerificationCodeSent == value)
                    return;
                SetProperty(ref _isVerificationCodeSent, value);
            }
        }

        public string two_factor_identifier { get; set; } = string.Empty;
        public string ChallengeUrl { get; set; } = string.Empty;

        private bool _isManualVerify;
        public bool IsManualVerify
        {
            get
            {
                return _isManualVerify;
            }
            set
            {
                if (value)
                    IsAutoVerifyByEmail = false;
                SetProperty(ref _isManualVerify, value);

            }
        }

        private bool _isRunProcessThroughBrowser = true;
        [ProtoMember(28)]
        public bool IsRunProcessThroughBrowser
        {
            get
            {
                return _isRunProcessThroughBrowser;
            }
            set
            {

                if (_isRunProcessThroughBrowser == value)
                    return;
                SetProperty(ref _isRunProcessThroughBrowser, value);
            }
        }

        public bool IsNeedToSchedule { get; set; } = true;
        public DominatorAccountModel Clone()
        {
            return (DominatorAccountModel)MemberwiseClone();
        }

        internal object Where(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}
