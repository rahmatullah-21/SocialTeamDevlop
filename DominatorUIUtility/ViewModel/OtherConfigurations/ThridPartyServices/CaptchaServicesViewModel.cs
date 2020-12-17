using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class CaptchaServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public CaptchaServicesViewModel(IGenericFileManager genericFileManager) : base("LangKeyCaptchaServices",
            "CaptchaServicesControlTemplate")
        {
            _genericFileManager = genericFileManager;
            CaptchaServicesModel =
                _genericFileManager.GetModel<CaptchaServicesModel>(ConstantVariable.GetCaptchaServicesFile()) ??
                new CaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        public CaptchaServicesModel CaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_genericFileManager.Save(CaptchaServicesModel, ConstantVariable.GetCaptchaServicesFile()))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyCaptchaSaved".FromResourceDictionary());
        }
    }
}