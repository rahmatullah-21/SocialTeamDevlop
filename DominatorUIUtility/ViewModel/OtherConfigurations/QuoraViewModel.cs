using CommonServiceLocator;
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
        private readonly IConstantVariable _constantVariable;

        public QuoraModel QuoraModel { get; }
        public DelegateCommand SaveCmd { get; }
        public QuoraViewModel(IGenericFileManager genericFileManager) : base("LangKeyQuora", "QuoraControlTemplate")
        {
            _genericFileManager = genericFileManager;
            _constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();
            SaveCmd = new DelegateCommand(Save);
            QuoraModel = _genericFileManager.GetModel<QuoraModel>(_constantVariable.GetOtherQuoraSettingsFile()) ??
                         new QuoraModel();
        }

        private void Save()
        {
            if (_genericFileManager.Overrride(QuoraModel, _constantVariable.GetOtherQuoraSettingsFile()))
                Dialog.ShowDialog("Success", "Quora Configuration sucessfully saved !!");
        }
    }
}
