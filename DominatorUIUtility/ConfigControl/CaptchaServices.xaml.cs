using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.Config;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for CaptchaServices.xaml
    /// </summary>
    public partial class CaptchaServices : UserControl, INotifyPropertyChanged
    {
        private CaptchaServices()
        {
            InitializeComponent();
            CaptchaServicesViewModel.CaptchaServicesModel =
                GenericFileManager.GetModel<CaptchaServicesModel>(ConstantVariable.GetCaptchaServicesFile()) ??
                CaptchaServicesViewModel.CaptchaServicesModel;
            MainGrid.DataContext = CaptchaServicesViewModel;
        }

        private static CaptchaServices instance;

        public static CaptchaServices GetSingeltonCaptchaServices()
        {
            return instance ?? (instance = new CaptchaServices());
        }


        private CaptchaServicesViewModel _captchaServicesViewModel = new CaptchaServicesViewModel();

        public CaptchaServicesViewModel CaptchaServicesViewModel
        {
            get
            {
                return _captchaServicesViewModel;
            }
            set
            {
                _captchaServicesViewModel = value;
                OnPropertyChanged(nameof(CaptchaServicesViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (GenericFileManager.Save(CaptchaServicesViewModel.CaptchaServicesModel, ConstantVariable.GetCaptchaServicesFile()))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Captcha Services sucessfully saved !!");

        }
    }
}
