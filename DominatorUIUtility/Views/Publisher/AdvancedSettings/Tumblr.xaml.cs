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
    /// Interaction logic for Tumblr.xaml
    /// </summary>
    public partial class Tumblr : UserControl,INotifyPropertyChanged
    {
        public Tumblr()
        {
            InitializeComponent();
            MainGrid.DataContext = TumblrViewModel;
        }
        static Tumblr ObjTumblr = null;
        public static Tumblr GetSingeltonTumblr()
        {
            if (ObjTumblr == null)
                ObjTumblr = new Tumblr();
            return ObjTumblr;
        }
        private TumblrViewModel _tumblrViewModel = new TumblrViewModel();

        public TumblrViewModel TumblrViewModel
        {
            get
            {
                return _tumblrViewModel;
            }
            set
            {
                _tumblrViewModel = value;
                OnPropertyChanged(nameof(TumblrViewModel));
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
