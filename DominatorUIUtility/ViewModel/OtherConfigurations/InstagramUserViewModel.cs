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
    public class InstagramUserViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;
        public InstagramUserModel InstagramUserModel { get; }
        public DelegateCommand SaveCmd { get; }
        public InstagramUserViewModel(IGenericFileManager genericFileManager) : base("LangKeyInstagram", "InstagramControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            InstagramUserModel =
                _genericFileManager.GetModel<InstagramUserModel>(ConstantVariable.GetOtherInstagramSettingsFile()) ??
                new InstagramUserModel();
        }

        private void Save()
        {
            if (_genericFileManager.Overrride(InstagramUserModel, ConstantVariable.GetOtherInstagramSettingsFile()))
                Dialog.ShowDialog("Success", "Instagram Configuration sucessfully saved !!");
        }
    }
}
