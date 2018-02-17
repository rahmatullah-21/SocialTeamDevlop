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
        private  HomeAutoActivityViewModel _homeAutoActivityViewModel= new HomeAutoActivityViewModel();

        public HomeAutoActivityViewModel HomeAutoActivityViewModel
        {
            get
            {
                return _homeAutoActivityViewModel;
            }
            set
            {
                _homeAutoActivityViewModel = value;                  
                //OnPropertyChanged(nameof(HomeAutoActivityViewModel));
            }
        }

        private HomeAutoActivity()
        {
            InitializeComponent();
            this.DataContext = HomeAutoActivityViewModel;
        }

        private static HomeAutoActivity homeAutoActivity;

        public static HomeAutoActivity GetSingletonHomeAutoActivity()
        {
            return homeAutoActivity ?? (homeAutoActivity = new HomeAutoActivity());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
