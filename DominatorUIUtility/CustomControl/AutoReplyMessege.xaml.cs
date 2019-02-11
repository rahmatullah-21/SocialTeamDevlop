using System;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AutoReplyMessege.xaml
    /// </summary>
    public partial class AutoReplyMessege : UserControl
    {
        private AutoReplyMessegeModel AutoReplyMessegeModel { get; set; }=new AutoReplyMessegeModel();
        MultiMessage MultiMessageForUserHasNotReplied = new MultiMessage();
        MultiMessage MultiMessageForUserHasReplied = new MultiMessage();
        Dialog dialog = new Dialog();
        public AutoReplyMessege()
        {
            InitializeComponent();
            MainGrid.DataContext = AutoReplyMessegeModel;
        }


        private void AutoReplyStatus_OnIsCheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void BtnSendIfUserHasNotReplied_OnClick(object sender, RoutedEventArgs e)
        {
            Window win = dialog.GetMetroWindow(MultiMessageForUserHasNotReplied, "Messages");
            win.Show();
        }

        private void BtnSendIfUserHasReplied_OnClick(object sender, RoutedEventArgs e)
        {
            Window win = dialog.GetMetroWindow(MultiMessageForUserHasReplied, "Messages");
            win.Show();
        }
    }
}
