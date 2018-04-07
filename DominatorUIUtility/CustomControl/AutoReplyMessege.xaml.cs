using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Models;
using DominatorUIUtility.Behaviours;

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
