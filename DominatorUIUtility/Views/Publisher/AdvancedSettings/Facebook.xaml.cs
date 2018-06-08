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
using DominatorHouseCore.ViewModel.AdvancedSettings;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook : UserControl, INotifyPropertyChanged
    {
        private Facebook()
        {
            InitializeComponent();
            MainGrid.DataContext = FacebookViewModel;

        }

        static Facebook ObjFacebook = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Facebook GetSingeltonFacebookObject()
        {
            if (ObjFacebook == null)
                ObjFacebook = new Facebook();
            return ObjFacebook;
        }
        private FacebookViewModel _facebookViewModel = new FacebookViewModel();

        public FacebookViewModel FacebookViewModel
        {
            get
            {
                return _facebookViewModel;
            }
            set
            {
                _facebookViewModel = value;
                OnPropertyChanged(nameof(FacebookViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
