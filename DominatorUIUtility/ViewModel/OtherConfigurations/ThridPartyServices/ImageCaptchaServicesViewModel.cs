using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class ImageCaptchaServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        private readonly IGenericFileManager _genericFileManager;
        public ImageCaptchaServicesModel ImageCaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; private set; }

        public IConstantVariable _constantVaribale { get; }

        public ImageCaptchaServicesViewModel(IGenericFileManager genericFileManager) : base("LangKeyImageCaptchaServices", "ImageCaptchaServices")
        {
            _genericFileManager = genericFileManager;
            _constantVaribale = ServiceLocator.Current.GetInstance<IConstantVariable>();
            ImageCaptchaServicesModel = _genericFileManager.GetModel<ImageCaptchaServicesModel>(_constantVaribale.GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        private void Save()
        {
            if (_genericFileManager.Save(ImageCaptchaServicesModel, _constantVaribale.GetImageCaptchaServicesFile()))
                Dialog.ShowDialog("Success", "Image Captcha Services sucessfully saved !!");
        }
    }
}