using DominatorHouseCore.Command;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using System.Windows.Input;
using System;
using DominatorUIUtility.CustomControl;
using DominatorHouseCore.Models.SocioPublisher;
using System.Collections.Generic;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using System.Linq;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Enums.FdQuery;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore.LogHelper;
using System.Text.RegularExpressions;

namespace DominatorUIUtility.ViewModel.SocioPublisher.AdvancedSettings
{
    public class FacebookViewModel : BindableBase
    {
        public FacebookViewModel()
        {
            SelectFriendsCommand = new BaseCommand<object>(SelectFriendsCanExecute, SelectFriendsCommandExecute);
            SelectPagesCommand = new BaseCommand<object>(SelectPagesCanExecute, SelectPagesCommandExecute);
            SaveFriendCommad = new BaseCommand<object>(SelectPagesCanExecute, SaveFriendExecute);
            SavePageCommad = new BaseCommand<object>(SelectPagesCanExecute, SavePageExecute);
        }

        private void SavePageExecute(object sender)
        {
            try
            {
                var pageUrlList = Regex.Split(FacebookModel.CustomPageUrl, "\r\n");
                pageUrlList.ForEach(x =>
                {
                    FacebookModel.ListCustomPageUrl.Add(x);
                });

                FacebookModel.ListCustomPageUrl = FacebookModel.ListCustomPageUrl.Distinct().ToList();
            }
            catch (Exception ex)
            {

            }
        }

        private void SaveFriendExecute(object sender)
        {
            try
            {
                var taggedUserList = Regex.Split(FacebookModel.CustomTaggedUser, "\r\n");
                taggedUserList.ForEach(x =>
                {
                    FacebookModel.ListCustomTaggedUser.Add(x);
                });

                FacebookModel.ListCustomTaggedUser = FacebookModel.ListCustomTaggedUser.Distinct().ToList();
            }
            catch (Exception ex)
            {

            }
        }

        private void SelectFriendsCommandExecute(object obj)
        {
            SelectAccountDetailsControl SelectAccountDetailsControl = null;

            var model = FacebookModel.SelectFriendsDetailsModel;

            List<FbEntityTypes> hiddenColumnList = new List<FbEntityTypes>();

            hiddenColumnList.Add(FbEntityTypes.Page);
            hiddenColumnList.Add(FbEntityTypes.Group);

            List<string> listAccountIds = AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.AccountId).ToList();

            if (FacebookModel.AccountFriendsPair.Count==0)
            {
                var selectedAccounts = FacebookModel.SelectFriendsDetailsModel.AccountFriendsPair.Select(y => y.Key).ToList();

                listAccountIds = listAccountIds.Where(x => selectedAccounts.All(y => y == x)).ToList();

                SelectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages");
            }
            else
            {
                SelectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, model);
            }
            
            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(SelectAccountDetailsControl, "Select Account Details");

            SelectAccountDetailsControl.BtnSave.Click += (senders, Events) =>
            {
                try
                {
                    FacebookModel.AccountFriendsPair.Clear();
                    FacebookModel.ListCustomTaggedUser.Clear();

                    model = SelectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model.ListSelectDestination.ForEach(x =>
                    {
                        var accountFriendspair = model.AccountFriendsPair.Where(y => y.Key == x.AccountId).ToList();

                        if (x.IsAccountSelected)
                        {
                            FacebookModel.AccountFriendsPair.AddRange(accountFriendspair);
                            FacebookModel.ListCustomTaggedUser.AddRange(accountFriendspair.Select(z => z.Value).ToList());
                            FacebookModel.ListCustomPageUrl = FacebookModel.ListCustomPageUrl.Distinct().ToList();
                            FacebookModel.ListCustomTaggedUser.ForEach(z =>
                            {
                                if (!FacebookModel.CustomTaggedUser.Contains(z))
                                    FacebookModel.CustomTaggedUser += z + "\r\n";
                            });
                        }
                        else if(model.AccountFriendsPair.Any(y=> y.Key==x.AccountId))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "", $"Destiation is selected but Account is not selected");
                        }

                    });
                  

                    window.Close();
                }
                catch (Exception ex)
                {

                }
            };

            window.ShowDialog();

            FacebookModel.SelectFriendsDetailsModel = SelectAccountDetailsControl.GetSelectAccountModel();
        }


        private void SelectPagesCommandExecute(object obj)
        {
            SelectAccountDetailsControl SelectAccountDetailsControl = null;

            var model = FacebookModel.SelectPageDetailsModel;

            List<FbEntityTypes> hiddenColumnList = new List<FbEntityTypes>();

            hiddenColumnList.Add(FbEntityTypes.Friend);
            hiddenColumnList.Add(FbEntityTypes.Group);

            List<string> listAccountIds = AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.AccountId).ToList();

            if (FacebookModel.SelectPageDetailsModel.AccountPagesBoardsPair.Count == 0)
            {
                var selectedAccounts = FacebookModel.SelectPageDetailsModel.AccountPagesBoardsPair.Select(y => y.Key).ToList();

                listAccountIds = listAccountIds.Where(x => selectedAccounts.All(y => y == x)).ToList();

                SelectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages", true);
            }
            else
            {
                SelectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, model, true);
            }

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(SelectAccountDetailsControl, "Select Account Details");

            SelectAccountDetailsControl.BtnSave.Click += (senders, Events) =>
            {
                try
                {
                    FacebookModel.AccountPagesBoardsPair.Clear();
                    FacebookModel.ListCustomPageUrl.Clear();

                    model = SelectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model.ListSelectDestination.ForEach(x =>
                    {
                        var accountPagespair = model.AccountPagesBoardsPair.Where(y => y.Key == x.AccountId).ToList();

                        if (x.IsAccountSelected)
                        {
                            FacebookModel.AccountPagesBoardsPair.AddRange(accountPagespair);
                            FacebookModel.ListCustomPageUrl.AddRange(accountPagespair.Select(z => z.Value).ToList());
                            FacebookModel.ListCustomPageUrl = FacebookModel.ListCustomPageUrl.Distinct().ToList();
                            FacebookModel.ListCustomPageUrl.ForEach(z =>
                            {
                                if (!FacebookModel.CustomPageUrl.Contains(z))
                                    FacebookModel.CustomPageUrl += z + "\r\n";
                            });
                        }
                        else if (model.AccountPagesBoardsPair.Any(y => y.Key == x.AccountId))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "", $"Destiation is selected but Account is not selected");
                        }

                    });


                    window.Close();
                }
                catch (Exception ex)
                {

                }
            };

            window.ShowDialog();

            FacebookModel.SelectPageDetailsModel = SelectAccountDetailsControl.GetSelectAccountModel();
        }

        private bool SelectFriendsCanExecute(object arg) => true;

        private bool SelectPagesCanExecute(object arg) => true;

        private bool SavePagesCanExecute(object arg) => true;

        private bool SaveFriendCanExecute(object arg) => true;


        public ICommand SelectFriendsCommand { get; set; }

        public ICommand SelectPagesCommand { get; set; }

        public ICommand SaveFriendCommad { get; set; }

        public ICommand SavePageCommad { get; set; }

        private FacebookModel _facebookModel = new FacebookModel();

        public FacebookModel FacebookModel
        {
            get
            {
                return _facebookModel;
            }
            set
            {
                if (_facebookModel == value)
                    return;
                SetProperty(ref _facebookModel, value);
            }
        }


        
    }
}
