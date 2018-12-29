using CommonServiceLocator;
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
        private readonly IGenericFileManager _genericFileManager;
        public UrlShortnerServicesModel UrlShortnerServicesModel { get; }
        public DelegateCommand SaveCmd { get; private set; }

        public UrlShortnerServicesViewModel(IGenericFileManager genericFileManager) : base("LangKeyUrlShortnerServices", "UrlShortnerServicesControlTemplate")
        {
            _genericFileManager = genericFileManager;
            IConstantVariable constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();
            UrlShortnerServicesModel = _genericFileManager.GetModel<UrlShortnerServicesModel>(constantVariable.GetURLShortnerServicesFile()) ?? new UrlShortnerServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        private void Save()
        {
            IConstantVariable constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();
            if (_genericFileManager.Save(UrlShortnerServicesModel, constantVariable.GetURLShortnerServicesFile()))
            {
                constantVariable.BitlyLogin = UrlShortnerServicesModel.Login;
                constantVariable.BitlyApiKey = UrlShortnerServicesModel.ApiKey;

                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Url Shortner Services sucessfully saved !!");
            }
        }
    }
}
