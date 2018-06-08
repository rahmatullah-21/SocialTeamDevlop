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
    /// Interaction logic for Twitter.xaml
    /// </summary>
    public partial class Twitter : UserControl,INotifyPropertyChanged
    {
        private Twitter()
        {
            InitializeComponent();
            MainGrid.DataContext = TwitterViewModel;
        }
        static Twitter ObjTwitter = null;
        public static Twitter GetSingletonTwitterObject()
        {
            if (ObjTwitter == null)
                ObjTwitter = new Twitter();
            return ObjTwitter;
        }
        private TwitterViewModel _twitterViewModel = new TwitterViewModel();

        public TwitterViewModel TwitterViewModel
        {
            get
            {
                return _twitterViewModel;
            }
            set
            {
                _twitterViewModel = value;
                OnPropertyChanged(nameof(TwitterViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
