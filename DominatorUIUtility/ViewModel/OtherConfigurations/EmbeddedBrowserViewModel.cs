using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class EmbeddedBrowserViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IOtherConfigFileManager _otherConfigFileManager;

        public EmbeddedBrowserSettingsModel EmbeddedBrowserModel { get; }
        public DelegateCommand SaveCmd { get; }
        public EmbeddedBrowserViewModel(IOtherConfigFileManager otherConfigFileManager) : base("LangKeyEmbeddedBrowserSettings", "EmbeddedBrowserControlTemplate")
        {
            _otherConfigFileManager = otherConfigFileManager;
            SaveCmd = new DelegateCommand(Save);
            EmbeddedBrowserModel = _otherConfigFileManager.GetOtherConfig<EmbeddedBrowserSettingsModel>() ??
                         new EmbeddedBrowserSettingsModel();
        }

        private void Save()
        {
            if (_otherConfigFileManager.SaveOtherConfig(EmbeddedBrowserModel))
                Dialog.ShowDialog("Success", "Embedded Browser Settings sucessfully saved !!");
        }
    }
}
