using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class LinkedInViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;
        public LinkedInModel LinkedInModel { get; }
        public DelegateCommand SaveCmd { get; }
        public LinkedInViewModel(IGenericFileManager genericFileManager) : base("LangKeyLinkedIn", "LinkedInControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            LinkedInModel =
                _genericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile()) ??
                new LinkedInModel();
        }

        private void Save()
        {
            if (_genericFileManager.Overrride(LinkedInModel, ConstantVariable.GetOtherLinkedInSettingsFile()))
                Dialog.ShowDialog("Success", "LinkedIn Configuration sucessfully saved !!");
        }
    }
}
