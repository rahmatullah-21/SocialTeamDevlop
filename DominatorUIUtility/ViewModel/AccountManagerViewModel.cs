using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Windows.Controls;

namespace DominatorUIUtility.ViewModel
{
    public class AccountManagerViewModel : BindableBase
    {
        private static AccountManagerViewModel ObjAccountManagerViewModel { get; set; }

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
