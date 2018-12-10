using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class QuoraViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;
        public QuoraModel QuoraModel { get; }
        public DelegateCommand SaveCmd { get; }
        public QuoraViewModel(IGenericFileManager genericFileManager) : base("LangKeyQuora", "QuoraControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            QuoraModel = _genericFileManager.GetModel<QuoraModel>(ConstantVariable.GetOtherQuoraSettingsFile()) ??
                         new QuoraModel();
        }

        private void Save()
        {
            if (_genericFileManager.Overrride(QuoraModel, ConstantVariable.GetOtherQuoraSettingsFile()))
                Dialog.ShowDialog("Success", "Quora Configuration sucessfully saved !!");
        }
    }
}
