using System;
using DominatorHouseCore.Utility;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace DominatorUIUtility.ViewModel
{
    public class AccountManagerViewModel : BindableBase
    {
        private static AccountManagerViewModel ObjAccountManagerViewModel { get; set; } = null;

        public static AccountManagerViewModel GetSingletonAccountManagerViewModel()
            => ObjAccountManagerViewModel ?? (ObjAccountManagerViewModel = new AccountManagerViewModel());

        private UserControl _selectedUserControl;

        public UserControl SelectedUserControl
        {
            get
            {
                return _selectedUserControl;
            }
            set
            {
                SetProperty(ref _selectedUserControl, value);
            }
        }

        public void CallRespectiveView(string controlType, [CanBeNull] DominatorAccountModel dominatorAccountModel, SocialNetworks network)
        {
            try
            {
                if (controlType == "AccountManager")
                {

                    var accountCustomControl = AccountCustomControl.GetAccountCustomControl(network);
                    SelectedUserControl = null;
                    SelectedUserControl = accountCustomControl;
                    accountCustomControl.GetRespectiveAccounts(network);

                }
                else
                    SelectedUserControl = new AccountDetail(dominatorAccountModel);
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}
