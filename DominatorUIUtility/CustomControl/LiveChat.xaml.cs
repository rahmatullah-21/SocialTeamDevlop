using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for LiveChat.xaml
    /// </summary>
    public partial class LiveChat : UserControl
    {
        public LiveChatViewModel LiveChatViewModel { get; set; }
        public LiveChat()
        {
            InitializeComponent();

            LiveChatViewModel = new LiveChatViewModel();

            #region SnderDetails
            LiveChatViewModel.LiveChatModel.LstSender = new List<SenderDetails>
            {
                new SenderDetails{
                    SenderImage=@"D:\DominatorHouse Development New GIT\dominatorhouse-social\DominatorUIUtility\Images\browsercheck.png",
                    SenderName="AQQ",
                    LastMessegedate=1520857863,
                    LastMesseges="Hi"
                },
                 new SenderDetails{
                    SenderImage=@"D:\DominatorHouse Development New GIT\dominatorhouse-social\DominatorUIUtility\Images\setting.png",
                    SenderName="B",
                    LastMessegedate=1520832687,
                    LastMesseges="Hi2"
                }, new SenderDetails{
                    SenderImage=@"C:\Users\GLB-259\Desktop\Tripadvisor Scraper\Tools_instagram_followback_sources.png",
                    SenderName="CQQ",
                    LastMessegedate=1520816427,
                    LastMesseges="Hi3"
                }

            };
            #endregion

            UpdateAccountChatName();

            MainGrid.DataContext = LiveChatViewModel.LiveChatModel;
            cmbAccounts.ItemsSource = AccountsFileManager.GetUsers();
        }

        public void UpdateAccountChatName()
        {
            var chatdetails = LiveChatFileManager.GetAllChatDetails();
            if (chatdetails.Count != 0)
                LiveChatViewModel.LiveChatModel.AccountChatDetails = chatdetails;
            else
                LiveChatViewModel.LiveChatModel.LstSender.ForEach(x =>
                {
                    try
                    {
                        LiveChatViewModel.LiveChatModel.AccountChatDetails.Add(x.SenderName, new ObservableCollection<ChatDetails>());
                    }
                    catch (Exception ex)
                    {

                    }
                });
        }


        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {

            var currentItem = new ChatDetails
            {
                Sender = cmbAccounts.SelectedValue.ToString(),
                Messeges = TxtMessege.Text,
                Time = DateTime.Now.ToString("hh:mm tt"),
                Type = "Sent",
            };

            LiveChatViewModel.LiveChatModel.LstChat.Add(currentItem);

            TxtMessege.Clear();
            TxtMessege.Focus();

            string SenderToUpdate= (Senders.SelectedItem as SenderDetails).SenderName;

            var ChatDetails = LiveChatViewModel.LiveChatModel.AccountChatDetails;

            if (ChatDetails.ContainsKey(SenderToUpdate))
            {
                try
                {
                    if (ChatDetails[SenderToUpdate] == null)
                    {
                        ChatDetails[SenderToUpdate] = new ObservableCollection<ChatDetails> { currentItem };
                    }
                    else
                        ChatDetails[SenderToUpdate].Add(currentItem);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
            }
           
            LiveChatFileManager.SaveLiveChat(ChatDetails);

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
                    var chatdetail = LiveChatFileManager.GetAllChatDetails().FirstOrDefault(x =>
                             x.Key == (Senders.SelectedItem as SenderDetails).SenderName).Value;

                    if (chatdetail != null)
                    {
                        var chatdetailCollection = new ObservableCollection<ChatDetails>(chatdetail);
                        var ChatsWithSelectedAccount = chatdetailCollection.Where(x => x.Sender == cmbAccounts.SelectedValue.ToString());
                        LiveChatViewModel.LiveChatModel.LstChat = new ObservableCollection<ChatDetails>(ChatsWithSelectedAccount);
                    }
                    else
                        LiveChatViewModel.LiveChatModel.LstChat = new ObservableCollection<ChatDetails>();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
            }
        }

        private void cmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetCurrentChat();
        }
    }
}
