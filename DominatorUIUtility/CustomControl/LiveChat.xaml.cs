using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;

namespace DominatorUIUtility.CustomControl
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    /// Interaction logic for LiveChat.xaml
    /// </summary>
    public partial class LiveChat : UserControl
    {
        public LiveChatViewModel LiveChatViewModel { get; set; }

        public Func<string, string, bool> SendMessage { get; set; }

        public Action<LiveChatModel> UpdateAccountChatList { get; set; }

        public Action<LiveChatModel> UpdatePerticularThread { get; set; }

        public SocialNetworks SocialNetworks { get; set; }

        public Func<LiveChatModel, string, bool> SendMessageToUser { get; set; }

        public LiveChat(SocialNetworks network, Action<LiveChatModel> updateAccountChatList = null, Action<LiveChatModel> updatePerticularThread = null, Func<LiveChatModel, string, bool> sendMessageToUser = null)
        {

            InitializeComponent();

            LiveChatViewModel = new LiveChatViewModel();

            UpdateAccountChatList = updateAccountChatList;

            SocialNetworks = network;

            UpdatePerticularThread = updatePerticularThread;

            SendMessageToUser = sendMessageToUser;

            MainGrid.DataContext = LiveChatViewModel.LiveChatModel;

            SendMessage = SendMessage;

            #region SnderDetails

            //LiveChatViewModel.LiveChatModel.LstSender = new List<SenderDetails>
            //{
            //    new SenderDetails{
            //        SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
            //        SenderName="AQQ",
            //        LastMessegedate="1520857863",
            //        LastMesseges="Hi"
            //    },
            //    new SenderDetails{
            //        SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
            //        SenderName="B",
            //        LastMessegedate="1520832687",
            //        LastMesseges="Hi2"
            //    },
            //    new SenderDetails{
            //        SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
            //        SenderName="CQQ",
            //        LastMessegedate="1520816427",
            //        LastMesseges="Hi3"
            //    }
            //};

            //LiveChatViewModel.LiveChatModel.SenderDetails = LiveChatViewModel.LiveChatModel.LstSender[0];

            #endregion

            var accountCustom = AccountCustomControl.GetAccountCustomControl(SocialNetworks);

            var accounts = accountCustom.DominatorAccountViewModel.LstDominatorAccountModel
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks).Select(x => x.UserName).ToList();

            LiveChatViewModel.LiveChatModel.AccountNames = new ObservableCollection<string>(accounts);

            if (LiveChatViewModel.LiveChatModel.AccountNames.Count > 0)
            {
                LiveChatViewModel.LiveChatModel.SelectedAccount = LiveChatViewModel.LiveChatModel.AccountNames.First();
            }
            try
            {
                LiveChatViewModel.LstAccountModel =
                    accountCustom.DominatorAccountViewModel.LstDominatorAccountModel.ToList();
                LiveChatViewModel.LiveChatModel.DominatorAccountModel =
                    LiveChatViewModel.LstAccountModel.FirstOrDefault(x =>
                        x.UserName == LiveChatViewModel.LstAccountModel[0].UserName.ToString());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public LiveChat(SocialNetworks network)
        {

            InitializeComponent();

            LiveChatViewModel = new LiveChatViewModel();
            SocialNetworks = network;
            MainGrid.DataContext = LiveChatViewModel.LiveChatModel;

            #region SnderDetails

            //LiveChatViewModel.LiveChatModel.LstSender = new List<SenderDetails>
            //{
            //    new SenderDetails{
            //        SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
            //        SenderName="AQQ",
            //        LastMessegedate="1520857863",
            //        LastMesseges="Hi"
            //    },
            //    new SenderDetails{
            //        SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
            //        SenderName="B",
            //        LastMessegedate="1520832687",
            //        LastMesseges="Hi2"
            //    },
            //    new SenderDetails{
            //        SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
            //        SenderName="CQQ",
            //        LastMessegedate="1520816427",
            //        LastMesseges="Hi3"
            //    }
            //};

            //LiveChatViewModel.LiveChatModel.SenderDetails = LiveChatViewModel.LiveChatModel.LstSender[0];

            #endregion

            var accountCustom = AccountCustomControl.GetAccountCustomControl(SocialNetworks);

            var accounts = accountCustom.DominatorAccountViewModel.LstDominatorAccountModel
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks).Select(x => x.UserName).ToList();

            LiveChatViewModel.LiveChatModel.AccountNames = new ObservableCollection<string>(accounts);

            if (LiveChatViewModel.LiveChatModel.AccountNames.Count > 0)
            {
                LiveChatViewModel.LiveChatModel.SelectedAccount = LiveChatViewModel.LiveChatModel.AccountNames.First();
            }
            try
            {
                LiveChatViewModel.LstAccountModel =
                    accountCustom.DominatorAccountViewModel.LstDominatorAccountModel.ToList();
                LiveChatViewModel.LiveChatModel.DominatorAccountModel =
                    LiveChatViewModel.LstAccountModel.FirstOrDefault(x =>
                        x.UserName == LiveChatViewModel.LstAccountModel[0].UserName.ToString());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void UpdateAccountChatName() =>
            UpdateAccountChatList?.Invoke(LiveChatViewModel.LiveChatModel);

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LiveChatViewModel.LiveChatModel.TextMessage))
            {
             bool isSent=   SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                    .SendMessageToUser(LiveChatViewModel.LiveChatModel, LiveChatViewModel.LiveChatModel.TextMessage,
                        MessageType.Text);
                if (isSent)
                {
                    LiveChatViewModel.LiveChatModel.TextMessage = string.Empty;
                }
                else
                {
                    MessageBox.Show("Got some error while sending message");
                }

                //if (SendMessageToUser(LiveChatViewModel.LiveChatModel, LiveChatViewModel.LiveChatModel.TextMessage))
                //{
                //    LiveChatViewModel.LiveChatModel.TextMessage = string.Empty;
                //}
                //else
                //{
                //    MessageBox.Show("Got some error while sending message");
                //}
            }
        }

        private void Senders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetCurrentChat();
        }

        private void GetCurrentChat()
        {
            if (LiveChatViewModel.LiveChatModel.SenderDetails != null)
            {
                try
                {
                    //var senderDetails = Senders.SelectedItem as SenderDetails;
                    //LiveChatViewModel.LiveChatModel.SenderDetails = senderDetails;
                    ThreadFactory.Instance.Start(() =>
                    {
                       SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                            .UpdateCurrentChat(LiveChatViewModel.LiveChatModel);


                      //  UpdatePerticularThread(LiveChatViewModel.LiveChatModel);
                    });
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
            }
        }

        private void cmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LiveChatViewModel.LiveChatModel.DominatorAccountModel =
                LiveChatViewModel.LstAccountModel.FirstOrDefault(x =>
                    x.UserName == LiveChatViewModel.LiveChatModel.SelectedAccount);

            ThreadFactory.Instance.Start(() =>
            {
                SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                    .UpdateFriendList(LiveChatViewModel.LiveChatModel);


              //  UpdateAccountChatList?.Invoke(LiveChatViewModel.LiveChatModel);
            });

            // ThreadFactory.Instance.Start(GetCurrentChat);
        }
    }
}
