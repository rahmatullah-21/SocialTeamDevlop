using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System.Windows;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class CaptchaServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        public CaptchaServicesModel CaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; private set; }

        public CaptchaServicesViewModel() : base("LangKeyCaptchaServices", "CaptchaServicesControlTemplate")
        {
            CaptchaServicesModel = GenericFileManager.GetModel<CaptchaServicesModel>(ConstantVariable.GetCaptchaServicesFile()) ?? new CaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        private void Save()
        {
            if (GenericFileManager.Save(CaptchaServicesModel, ConstantVariable.GetCaptchaServicesFile()))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Captcha Services sucessfully saved !!");
        }
    }
}
