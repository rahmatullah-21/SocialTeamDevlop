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
    /// Interaction logic for OtherConfig.xaml
    /// </summary>
    public partial class OtherConfig : UserControl
    {
        public MultiMessage MultiMessageForUserHasNotReplied = new MultiMessage();
        public MultiMessage MultiMessageForUserHasReplied = new MultiMessage();
        Dialog dialog = new Dialog();

        public OtherConfig()
        {
            InitializeComponent();
            OtherConfigFilter = new OtherConfigModel();
            MainGrid.DataContext = this;
        }
        public OtherConfigModel OtherConfigFilter
        {
            get { return (OtherConfigModel)GetValue(OtherConfigProperty); }
            set { SetValue(OtherConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OtherConfigProperty =
            DependencyProperty.Register("OtherConfigFilter", typeof(OtherConfigModel), typeof(OtherConfig), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void BtnSendIfUserHasReplied_OnClick(object sender, RoutedEventArgs e)
        {
            Window win = dialog.GetMetroWindow(MultiMessageForUserHasReplied, "Messages for user who has replied");
            win.Show();
        }

        private void BtnSendIfUserHasNotReplied_OnClick(object sender, RoutedEventArgs e)
        {
            Window win = dialog.GetMetroWindow(MultiMessageForUserHasNotReplied, "Messages for user who has not replied");
            win.Show();
        }
    }
}
