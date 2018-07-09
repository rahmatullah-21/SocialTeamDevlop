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
    /// Interaction logic for URLShortnerServices.xaml
    /// </summary>
    public partial class URLShortnerServices : UserControl,INotifyPropertyChanged
    {
        private URLShortnerServices()
        {
            InitializeComponent();
            UrlShortnerServicesViewModel.UrlShortnerServicesModel = GenericFileManager.GetModel<UrlShortnerServicesModel>(ConstantVariable.GetURLShortnerServicesFile()) ?? UrlShortnerServicesViewModel.UrlShortnerServicesModel;
            MainGrid.DataContext = UrlShortnerServicesViewModel;
        }

        private static URLShortnerServices instance;

        public static URLShortnerServices GetSingeltonUrlShortnerServices()
        {
            return instance ?? (instance = new URLShortnerServices());
        }
        private UrlShortnerServicesViewModel _urlShortnerServicesViewModel=new UrlShortnerServicesViewModel();

        public UrlShortnerServicesViewModel UrlShortnerServicesViewModel
        {
            get
            {
                return _urlShortnerServicesViewModel;
            }
            set
            {
                _urlShortnerServicesViewModel = value;
                OnPropertyChanged(nameof(UrlShortnerServicesViewModel));
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
            if (GenericFileManager.Save(UrlShortnerServicesViewModel.UrlShortnerServicesModel, ConstantVariable.GetURLShortnerServicesFile()))
            {
                ConstantVariable.BitlyLogin = UrlShortnerServicesViewModel.UrlShortnerServicesModel.Login;
                ConstantVariable.BitlyApiKey = UrlShortnerServicesViewModel.UrlShortnerServicesModel.ApiKey;

                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Url Shortner Services sucessfully saved !!");
            }
        }
    }
}
