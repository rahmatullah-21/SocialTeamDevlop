using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class LinkedInViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        public LinkedInModel LinkedInModel { get; }
        public DelegateCommand SaveCmd { get; }
        public LinkedInViewModel() : base("LangKeyLinkedIn", "LinkedInControlTemplate")
        {
            SaveCmd = new DelegateCommand(Save);
            LinkedInModel =
                GenericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile()) ??
                new LinkedInModel();
        }

        private void Save()
        {
            if (GenericFileManager.Overrride(LinkedInModel, ConstantVariable.GetOtherLinkedInSettingsFile()))
                Dialog.ShowDialog("Success", "LinkedIn Configuration sucessfully saved !!");
        }
    }
}
