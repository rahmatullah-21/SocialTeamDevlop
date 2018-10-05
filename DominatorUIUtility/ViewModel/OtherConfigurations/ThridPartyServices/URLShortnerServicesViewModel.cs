using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System.Windows;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class UrlShortnerServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        public UrlShortnerServicesModel UrlShortnerServicesModel { get; }
        public DelegateCommand SaveCmd { get; private set; }

        public UrlShortnerServicesViewModel() : base("LangKeyUrlShortnerServices", "UrlShortnerServicesControlTemplate")
        {
            UrlShortnerServicesModel = GenericFileManager.GetModel<UrlShortnerServicesModel>(ConstantVariable.GetURLShortnerServicesFile()) ?? new UrlShortnerServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        private void Save()
        {
            if (GenericFileManager.Save(UrlShortnerServicesModel, ConstantVariable.GetURLShortnerServicesFile()))
            {
                ConstantVariable.BitlyLogin = UrlShortnerServicesModel.Login;
                ConstantVariable.BitlyApiKey = UrlShortnerServicesModel.ApiKey;

                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Url Shortner Services sucessfully saved !!");
            }
        }
    }
}
