using DominatorHouseCore;
using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
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
      
    }
}
