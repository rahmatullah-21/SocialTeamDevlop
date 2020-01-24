using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using LegionUIUtility.ViewModel.SocioPublisher;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountDetailsSelector.xaml
    /// </summary>
    public partial class AccountDetailsSelector : UserControl, INotifyPropertyChanged
    {
        public AccountDetailsSelector(Func<string, string, AccountDetailsSelector, Task> updateUiData,
              string accountId, string accountName, bool isPageOptionVisible = false)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            AccountDetailsSelectorViewModel.IsPageOptionVisible = isPageOptionVisible;
            _accountId = accountId;
            _accountName = accountName;
            _updateUiDetails = updateUiData;
        }


        public AccountDetailsSelector(Action<AccountDetailsSelector> updateAllData
            , string detailsType = "")
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            AccountDetailsSelectorViewModel.IsPageOptionVisible = detailsType == "Page" ? true : false;
            AccountDetailsSelectorViewModel.IsGroupOptionVisible = detailsType == "Group" ? true : false;
            _updateAllDetails = updateAllData;
        }


        public AccountDetailsSelector(Func<AccountDetailsSelector, PublisherCreateDestinationSelectModel, Task> updateSingleData,
            PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel, string detailsType = "")
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            AccountDetailsSelectorViewModel.IsPageOptionVisible = detailsType == "Page" ? true : false;
            AccountDetailsSelectorViewModel.IsGroupOptionVisible = detailsType == "Group" ? true : false;
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
