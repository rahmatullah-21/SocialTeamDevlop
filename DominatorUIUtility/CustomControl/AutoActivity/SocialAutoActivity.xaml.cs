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
using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel.AutoActivity;

namespace DominatorUIUtility.CustomControl.AutoActivity
{
    /// <summary>
    /// Interaction logic for SocialAutoActivity.xaml
    /// </summary>
    public partial class SocialAutoActivity : UserControl
    {

        public SocialAutoActivity()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var homeAutoActivity = HomeAutoActivity.GetSingletonHomeAutoActivity();

            homeAutoActivity.HomeAutoActivityViewModel.UserControlSwitchViewModel= new DominatorAutoActivityViewModel(SocialNetworks.Instagram);
        }

        private static SocialAutoActivity socialAutoActivity;

        public static SocialAutoActivity GetSingletonSocialAutoActivity()
        {
            return socialAutoActivity ?? (socialAutoActivity = new SocialAutoActivity());
        }
    }
}
