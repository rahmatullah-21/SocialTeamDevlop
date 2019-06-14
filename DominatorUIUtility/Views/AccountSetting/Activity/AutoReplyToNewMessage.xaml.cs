using DominatorHouseCore;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for AutoReplyToNewMessage.xaml
    /// </summary>
    public partial class AutoReplyToNewMessage : UserControl
    {
        IAutoReplyToNewMessageViewModel ObjViewModel;
        public AutoReplyToNewMessage(IAutoReplyToNewMessageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ObjViewModel = viewModel;
        }
        private void ChkMessagesContainsSpecificWords_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                //ObjViewModel.SpecificWord = string.Empty;
                //int count = ObjViewModel.ManageMessagesModel.LstQueries.Count;
                //while (count > 1)
                //{
                //    var Content = ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries[count - 1]
                //        .Content;
                //    if (Content.QueryValue != "Default" &&
                //        Content.QueryValue != FindResource("LangKeyReplyToAllMessages").ToString())
                //    {
                //        ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.RemoveAt(count - 1);
                //    }

                //    count--;
                //}
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
