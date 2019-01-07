using CommonServiceLocator;
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
        public CaptchaServicesModel CaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; private set; }

        public IConstantVariable _constantVariable { get; set; }

        public CaptchaServicesViewModel(IGenericFileManager genericFileManager) : base("LangKeyCaptchaServices", "CaptchaServicesControlTemplate")
        {
            _genericFileManager = genericFileManager;
            _constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();
            CaptchaServicesModel = _genericFileManager.GetModel<CaptchaServicesModel>(_constantVariable.GetCaptchaServicesFile()) ?? new CaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        private void Save()
        {
            if (_genericFileManager.Save(CaptchaServicesModel, _constantVariable.GetCaptchaServicesFile()))
                Dialog.ShowDialog("Success", "Captcha Services sucessfully saved !!");

        }
    }
}
