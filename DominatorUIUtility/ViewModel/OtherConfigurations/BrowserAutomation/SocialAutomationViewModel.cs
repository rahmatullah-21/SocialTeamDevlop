using System;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.ViewModel;
using Prism.Commands;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Diagnostics;
using System.Linq;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class SocialAutomationViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IAccountsFileManager _accountsFileManager;

        public IBrowserAutomationModel BrowserAutomationModel { get; }

        public DelegateCommand SaveCmd { get; private set; }

        public SocialAutomationViewModel(IAccountsFileManager accountsFileManager, IBrowserAutomationModel
            browserAutomationModel) : base("LangKeyBrowserAutomation", "BrowserAutomationControlTemplate")
        {
            _accountsFileManager = accountsFileManager;
            BrowserAutomationModel = browserAutomationModel;
            SaveCmd = new DelegateCommand(Save);
            InitialiseAccounts();
        }

        private void InitialiseAccounts()
        {
            BrowserAutomationModel.ListSocialAccounts = new ObservableCollection<DominatorAccountModel>(_accountsFileManager.GetAll());
            BrowserAutomationModel.ListSocialNetworks = new ObservableCollection<string>(SocinatorInitialize.AvailableNetworks.Select(x=> x.ToString()).ToList());
        }

        private void Save()
        {
            //if (_genericFileManager.Save(CaptchaServicesModel, ConstantVariable.GetCaptchaServicesFile()))
            //    Dialog.ShowDialog("Success", "Captcha Services sucessfully saved !!");
        }
    }
}
