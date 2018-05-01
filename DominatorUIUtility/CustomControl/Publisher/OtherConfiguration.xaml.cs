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
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Views.Publisher;

namespace DominatorUIUtility.CustomControl.Publisher
{
    /// <summary>
    /// Interaction logic for OtherConfiguration.xaml
    /// </summary>
    public partial class OtherConfiguration : UserControl
    {
        public OtherConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }
        public OtherConfigurationModel OtherConfigurations
        {
            get { return (OtherConfigurationModel)GetValue(OtherConfigurationsProperty); }
            set { SetValue(OtherConfigurationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OtherConfigurationsProperty =
            DependencyProperty.Register("OtherConfigurations", typeof(OtherConfigurationModel), typeof(OtherConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        private void btnAdvancedSettings_Click(object sender, RoutedEventArgs e)
        {
            CampaignsAdvanceSetting ObjCampaignsAdvanceSetting = new CampaignsAdvanceSetting();
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(ObjCampaignsAdvanceSetting, "Campaign - Advanced Settings");
            window.Show();
        }
    }
}
