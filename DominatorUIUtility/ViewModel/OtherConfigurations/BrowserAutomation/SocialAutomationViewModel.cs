using System;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.ViewModel;
using Prism.Commands;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Diagnostics;
using System.Linq;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Diagnostics;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class SocialAutomationViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IAccountsFileManager _accountsFileManager;

        public IBrowserAutomationModel BrowserAutomationModel { get; }

        public DelegateCommand SaveCmd { get; private set; }

        public DelegateCommand NetWorkChangedCommand { get; private set; }




        public SocialAutomationViewModel(IAccountsFileManager accountsFileManager, IBrowserAutomationModel
            browserAutomationModel) : base("LangKeyBrowserAutomation", "BrowserAutomationControlTemplate")
        {
            _accountsFileManager = accountsFileManager;
            BrowserAutomationModel = browserAutomationModel;
            SaveCmd = new DelegateCommand(Save);
            InitialiseAccounts();
        }

        private void UpdateAccountList()
        {
            BrowserAutomationModel.ListSocialAccounts = new ObservableCollection<DominatorAccountModel>(_accountsFileManager.GetAll());
        }

        private void InitialiseAccounts()
        {
            BrowserAutomationModel.ListSocialAccounts = new ObservableCollection<DominatorAccountModel>(_accountsFileManager.GetAll());
            BrowserAutomationModel.ListSocialNetworks = new ObservableCollection<string>(SocinatorInitialize.AvailableNetworks.Select(x => x.ToString()).ToList());
        }

        private void Save()
        {
            BrowserAutomationModel.ListSocialAccounts.ForEach(x =>
            {
                new SocinatorAccountBuilder(x.AccountBaseModel.AccountId)
                   .AddOrUpdateBrowserSettings(x.IsRunProcessThroughBrowser)
                   .SaveToBinFile();
            });

            var result = Dialog.ShowCustomDialog("Success",
                    "Software Settings sucessfully saved.To apply this setting you need to restart.\nDo you want to Restart?", "Restart now", "Restart later");
            if (result == MessageDialogResult.Affirmative)
            {
              
                Application.Current.Shutdown();
                Process.Start(Application.ResourceAssembly.Location);
                Process.GetCurrentProcess().Kill();
                Environment.Exit(0);
            }            
        }
    }
}
