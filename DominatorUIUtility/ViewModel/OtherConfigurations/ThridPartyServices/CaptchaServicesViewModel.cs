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
        private readonly IGenericFileManager _genericFileManager;
        public CaptchaServicesModel CaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; private set; }

        public CaptchaServicesViewModel(IGenericFileManager genericFileManager) : base("LangKeyCaptchaServices", "CaptchaServicesControlTemplate")
        {
            _genericFileManager = genericFileManager;
            CaptchaServicesModel = _genericFileManager.GetModel<CaptchaServicesModel>(ConstantVariable.GetCaptchaServicesFile()) ?? new CaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        private void Save()
        {
            if (_genericFileManager.Save(CaptchaServicesModel, ConstantVariable.GetCaptchaServicesFile()))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Captcha Services sucessfully saved !!");
        }
    }
}
