using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AddUpdateAccountControl.xaml
    /// </summary>
    public partial class AccountDetail : UserControl, INotifyPropertyChanged
    {

        private AccountDetailsViewModel _accountDetailsViewModel;

        public AccountDetailsViewModel AccountDetailsViewModel
        {
            get { return _accountDetailsViewModel; }
            set
            {
                _accountDetailsViewModel = value;
                OnPropertyChanged(nameof(AccountDetailsViewModel));
            }
        }

        /// <summary>
        /// Constructor with default data context
        /// </summary>
        public AccountDetail()
        {
            InitializeComponent();
        }

        public AccountDetail(DominatorAccountModel dataContext) : this()
        {
            AccountDetailsViewModel = new AccountDetailsViewModel(dataContext);
            this.DataContext = AccountDetailsViewModel;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSave.IsDefault = true;
            }
        }
        private void OnVerificationKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnVerifyAccount.IsDefault = true;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

