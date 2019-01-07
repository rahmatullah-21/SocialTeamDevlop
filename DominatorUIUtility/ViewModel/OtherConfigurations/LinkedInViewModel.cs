using CommonServiceLocator;
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

        private readonly IConstantVariable _constantVariable;

        public LinkedInModel LinkedInModel { get; }
        public DelegateCommand SaveCmd { get; }
        public LinkedInViewModel(IGenericFileManager genericFileManager) : base("LangKeyLinkedIn", "LinkedInControlTemplate")
        {
            _genericFileManager = genericFileManager;
            _constantVariable= ServiceLocator.Current.GetInstance<IConstantVariable>();
            SaveCmd = new DelegateCommand(Save);
            LinkedInModel =
                _genericFileManager.GetModel<LinkedInModel>(_constantVariable.GetOtherLinkedInSettingsFile()) ??
                new LinkedInModel();
        }

        private void Save()
        {
            if (_genericFileManager.Overrride(LinkedInModel, _constantVariable.GetOtherLinkedInSettingsFile()))
                Dialog.ShowDialog("Success", "LinkedIn Configuration sucessfully saved !!");
        }
    }
}
