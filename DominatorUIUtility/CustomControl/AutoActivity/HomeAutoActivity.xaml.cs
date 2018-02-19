using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using DominatorHouseCore.Annotations;
using DominatorHouseCore.ViewModel.AutoActivity;

namespace DominatorUIUtility.CustomControl.AutoActivity
{
    /// <summary>
    /// Interaction logic for HomeAutoActivity.xaml
    /// </summary>
    public partial class HomeAutoActivity : UserControl 
    { 
             
        public HomeAutoActivityViewModel HomeAutoActivityViewModel { get; set; } = new HomeAutoActivityViewModel();

        private HomeAutoActivity()
        {
            InitializeComponent();
            HomeActivityPage.DataContext = HomeAutoActivityViewModel;
        }

        private static HomeAutoActivity homeAutoActivity;

        public static HomeAutoActivity GetSingletonHomeAutoActivity()
        {
            return homeAutoActivity ?? (homeAutoActivity = new HomeAutoActivity());
        }   
         
    }
}
