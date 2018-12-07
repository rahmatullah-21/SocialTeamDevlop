using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class ImageCaptchaServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        public ImageCaptchaServicesModel ImageCaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; private set; }

        public ImageCaptchaServicesViewModel() : base("LangKeyImageCaptchaServices", "ImageCaptchaServices")
        {
            ImageCaptchaServicesModel = GenericFileManager.GetModel<ImageCaptchaServicesModel>(ConstantVariable.GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        private void Save()
        {
            if (GenericFileManager.Save(ImageCaptchaServicesModel, ConstantVariable.GetImageCaptchaServicesFile()))
                Dialog.ShowDialog("Success","Image Captcha Services sucessfully saved !!");
        }
    }
}