using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;


namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class YoutubeViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;
        public YoutubeModel YoutubeModel { get; }
        public DelegateCommand SaveCmd { get; }
        public YoutubeViewModel(IGenericFileManager genericFileManager) : base("LangKeyYoutube", "YoutubeControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            YoutubeModel = _genericFileManager.GetModel<YoutubeModel>(ConstantVariable.GetOtherYoutubeSettingsFile()) ??
                         new YoutubeModel();
        }

        private void Save()
        {
            if (_genericFileManager.Overrride(YoutubeModel, ConstantVariable.GetOtherYoutubeSettingsFile()))
                Dialog.ShowDialog("Success", "Youtube Configuration sucessfully saved !!");
        }
    }
}

