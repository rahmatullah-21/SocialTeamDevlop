using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;


namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountCustomControl.xaml
    /// </summary>
    public partial class AccountCustomControl : INotifyPropertyChanged
    {
        private DominatorAccountViewModel _dominatorAccountViewModel;

        #region Property

        public DominatorAccountViewModel DominatorAccountViewModel
        {
            get
            {
                return _dominatorAccountViewModel;
            }
            set
            {
                _dominatorAccountViewModel = value;
                OnPropertyChanged(nameof(DominatorAccountViewModel));
            }
        }

        #endregion

        private AccountCustomControl()
        {
            _accountCustomInstance = this;
            _dominatorAccountViewModel = (DominatorAccountViewModel)ServiceLocator.Current.GetInstance<IDominatorAccountViewModel>();
            InitializeComponent();
            AccountModule.DataContext = DominatorAccountViewModel;
        }

        private static AccountCustomControl _accountCustomInstance;

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNetworks, AccessorStrategies strategies)
        {
            if (_accountCustomInstance == null)
                _accountCustomInstance = new AccountCustomControl();

            UncheckAll();
            return _accountCustomInstance;
        }

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNework)
        {
            UncheckAll();
            return _accountCustomInstance ?? (_accountCustomInstance = new AccountCustomControl());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void UncheckAll()
        {
            ServiceLocator.Current.GetInstance<IAccountCollectionViewModel>().GetCopySync().ForEach(x =>
            {
                x.IsAccountManagerAccountSelected = false;
            });
        }

    }
}
