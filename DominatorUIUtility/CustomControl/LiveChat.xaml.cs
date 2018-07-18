using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.EntitySql;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for LiveChat.xaml
    /// </summary>
    public partial class LiveChat : UserControl
    {
        public LiveChatViewModel LiveChatViewModel { get; set; }

        public Func<string, string, bool> sendMessage { get; set; }

        public Action<LiveChatModel> UpdateAccountChatList { get; set; }

        public Action<LiveChatModel> UpdatePerticularThread { get; set; }


        public SocialNetworks SocialNetworks { get; set; }

        public Func<LiveChatModel, string, bool> SendMessageToUser { get; set; }


        public LiveChat(SocialNetworks network, Action<LiveChatModel> UpdateAccountChatList = null, Action<LiveChatModel> UpdatePerticularThread = null, Func<LiveChatModel, string, bool> SendMessageToUser = null)
        {

            InitializeComponent();


            LiveChatViewModel = new LiveChatViewModel();


            this.UpdateAccountChatList = UpdateAccountChatList;

            SocialNetworks = network;

            this.UpdatePerticularThread = UpdatePerticularThread;

            this.SendMessageToUser = SendMessageToUser;

            MainGrid.DataContext = LiveChatViewModel.LiveChatModel;


            #region SnderDetails
            LiveChatViewModel.LiveChatModel.LstSender = new List<SenderDetails>
            {
                new SenderDetails{
                    SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
                    SenderName="AQQ",
                    LastMessegedate="1520857863",
                    LastMesseges="Hi"
                },
                new SenderDetails{
                    SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
                    SenderName="B",
                    LastMessegedate="1520832687",
                    LastMesseges="Hi2"
                },
                new SenderDetails{
                    SenderImage=@"C:\Users\Public\Pictures\Sample Pictures\1.jpg",
                    SenderName="CQQ",
                    LastMessegedate="1520816427",
                    LastMesseges="Hi3"
                }

            };
            #endregion


            this.sendMessage = sendMessage;


            AccountCustomControl accountCustom = AccountCustomControl.GetAccountCustomControl(SocialNetworks);

            var accoutns = accountCustom.DominatorAccountViewModel.LstDominatorAccountModel
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks).Select(x => x.UserName).ToList();


            LiveChatViewModel.LiveChatModel.AccountNames = new ObservableCollection<string>(accoutns);

            try
            {
                LiveChatViewModel.lstAccountModel =
                    accountCustom.DominatorAccountViewModel.LstDominatorAccountModel.ToList();
                LiveChatViewModel.LiveChatModel.dominatorAccountModel =
                    LiveChatViewModel.lstAccountModel.FirstOrDefault(x =>
                        x.UserName == LiveChatViewModel.lstAccountModel[0].UserName.ToString());
            }
            catch (Exception)
            {
            }

        }

        public void UpdateAccountChatName()
        {
            if (this.UpdateAccountChatList != null)
                this.UpdateAccountChatList(this.LiveChatViewModel.LiveChatModel);


        }


        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty((TxtMessege.Text)))
            {
                if (this.SendMessageToUser(this.LiveChatViewModel.LiveChatModel, TxtMessege.Text))
                {
                    TxtMessege.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Got some error while sending message");
                }
            }

            #region commented
            //var currentItem = new ChatDetails
            //{
            //    Sender = cmbAccounts.SelectedValue.ToString(),
            //    Messeges = TxtMessege.Text,
            //    Time = DateTime.Now.ToString("hh:mm tt"),
            //    Type = "Sent",
            //};

            //sendMessage(TxtMessege.Text, cmbAccounts.SelectedValue.ToString());

            //LiveChatViewModel.LiveChatModel.LstChat.Add(currentItem);

            //TxtMessege.Clear();
            //TxtMessege.Focus();

            //string SenderToUpdate = (Senders.SelectedItem as SenderDetails).SenderName;

            //var ChatDetails = LiveChatViewModel.LiveChatModel.AccountChatDetails;

            //if (ChatDetails.ContainsKey(SenderToUpdate))
            //{
            //    try
            //    {
            //        if (ChatDetails[SenderToUpdate] == null)
            //        {
            //            ChatDetails[SenderToUpdate] = new ObservableCollection<ChatDetails> { currentItem };
            //        }
            //        else
            //            ChatDetails[SenderToUpdate].Add(currentItem);
            //    }
            //    catch (Exception ex)
            //    {
            //        GlobusLogHelper.log.Error(ex.StackTrace);
            //    }
            //}

            //LiveChatFileManager.SaveLiveChat(ChatDetails); 
            #endregion
        }

        private void Senders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetCurrentChat();
        }

        private void GetCurrentChat()
        {
            if (Senders.SelectedItem != null)
            {
                try
                {
                    SenderDetails senderDetails = Senders.SelectedItem as SenderDetails;
                    LiveChatViewModel.LiveChatModel.SenderDetails = senderDetails;
                    this.UpdatePerticularThread(LiveChatViewModel.LiveChatModel);

                    #region commented
                    //var chatdetail = LiveChatFileManager.GetAllChatDetails().FirstOrDefault(x =>
                    //         x.Key == (Senders.SelectedItem as SenderDetails).SenderName).Value;

                    //if (chatdetail != null)
                    //{
                    //    var chatdetailCollection = new ObservableCollection<ChatDetails>(chatdetail);
                    //    var ChatsWithSelectedAccount = chatdetailCollection.Where(x => x.Sender == cmbAccounts.SelectedValue.ToString());
                    //    LiveChatViewModel.LiveChatModel.LstChat = new ObservableCollection<ChatDetails>(ChatsWithSelectedAccount);
                    //}
                    //else
                    //    LiveChatViewModel.LiveChatModel.LstChat = new ObservableCollection<ChatDetails>(); 
                    #endregion

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
            }
        }

        private void cmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LiveChatViewModel.LiveChatModel.dominatorAccountModel =
                LiveChatViewModel.lstAccountModel.FirstOrDefault(x =>
                    x.UserName == cmbAccounts.SelectedValue.ToString());

           // UpdateAccountChatList?.Invoke(LiveChatViewModel.LiveChatModel);
            ThreadFactory.Instance.Start(() => { UpdateAccountChatList?.Invoke(LiveChatViewModel.LiveChatModel); });

            // GetCurrentChat();
        }
    }
}
