using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountDetailsSelector.xaml
    /// </summary>
    public partial class AccountDetailsSelector : UserControl, INotifyPropertyChanged
    {
        public AccountDetailsSelector(Func<string, string, AccountDetailsSelector,Task> updateUiData, string accountId, string accountName)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            _accountId = accountId;
            _accountName = accountName;
            _updateUiDetails = updateUiData;           
        }


        public AccountDetailsSelector(Action<AccountDetailsSelector> updateAllData)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            _updateAllDetails = updateAllData;
        }


        public AccountDetailsSelector(Func<AccountDetailsSelector, PublisherCreateDestinationSelectModel, Task> updateSingleData, PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            _updateSinlgeDetails = updateSingleData;
            _publisherCreateDestinationSelectModel = publisherCreateDestinationSelectModel;
        }

        private readonly string _accountId;
        private readonly string _accountName;
        private PublisherCreateDestinationSelectModel _publisherCreateDestinationSelectModel = new PublisherCreateDestinationSelectModel();

        private readonly Func<string, string, AccountDetailsSelector,Task> _updateUiDetails;

        private readonly Action<AccountDetailsSelector> _updateAllDetails;

        private readonly Func<AccountDetailsSelector, PublisherCreateDestinationSelectModel, Task> _updateSinlgeDetails;

        private AccountDetailsSelectorViewModel _accountDetailsSelectorViewModel = new AccountDetailsSelectorViewModel();

        public AccountDetailsSelectorViewModel AccountDetailsSelectorViewModel
        {
            get
            {
                return _accountDetailsSelectorViewModel;
            }
            set
            {
                if (AccountDetailsSelectorViewModel == value)
                    return;
                _accountDetailsSelectorViewModel = value;
                OnPropertyChanged(nameof(AccountDetailsSelectorViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateUi() => ThreadFactory.Instance.Start(() =>
        {
            _updateUiDetails.Invoke(_accountId, _accountName, this);           
        });


        public void UpdateUiAllData() => ThreadFactory.Instance.Start(() =>
        {
            _updateAllDetails.Invoke(this);
        });

        public void UpdateUiSingleData() => ThreadFactory.Instance.Start(() =>
        {
            _updateSinlgeDetails.Invoke(this, _publisherCreateDestinationSelectModel);
        });
    }
}
