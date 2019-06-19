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
    public partial class AccountCustomControl : UserControl, INotifyPropertyChanged
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

            ServiceLocator.Current.GetInstance<IAccountCollectionViewModel>().GetCopySync().ForEach(x =>
            {
                x.IsAccountManagerAccountSelected = false;
            });
            return _accountCustomInstance;
        }

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNework)
        {
            return _accountCustomInstance ?? (_accountCustomInstance = new AccountCustomControl());
        }
      
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private void SelectActivity(object sender, RoutedEventArgs e)
        //{
        //    DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
        //    if (dominatorAccountModel != null)
        //    {
        //        Dialog dialog = new Dialog();
        //        var window = dialog.GetMetroWindow(new SaveSetting(dominatorAccountModel.AccountBaseModel.AccountNetwork), "Startup");
        //        //window.WindowStartupLocation = WindowStartupLocation.Manual;
        //        //window.Top = 0;
        //        //window.Left = 0;
        //        //window.HorizontalContentAlignment = HorizontalAlignment.Center;
        //        //window.VerticalContentAlignment = VerticalAlignment.Center;
        //        //window.MinHeight = SystemParameters.PrimaryScreenHeight - 100;
        //        //window.MinWidth = SystemParameters.PrimaryScreenWidth - 100;
        //        window.ShowDialog();
        //    }
        //}
    }
}
