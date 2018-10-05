using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class QuoraViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        public QuoraModel QuoraModel { get; }
        public DelegateCommand SaveCmd { get; }
        public QuoraViewModel() : base("LangKeyQuora", "QuoraControlTemplate")
        {
            SaveCmd = new DelegateCommand(Save);
            QuoraModel = GenericFileManager.GetModel<QuoraModel>(ConstantVariable.GetOtherQuoraSettingsFile()) ??
                         new QuoraModel();
        }

        private void Save()
        {
            if (GenericFileManager.Overrride(QuoraModel, ConstantVariable.GetOtherQuoraSettingsFile()))
                Dialog.ShowDialog("Success", "Quora Configuration sucessfully saved !!");
        }
    }
}
