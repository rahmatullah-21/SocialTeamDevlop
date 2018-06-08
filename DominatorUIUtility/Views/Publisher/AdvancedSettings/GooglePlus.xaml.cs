using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.ViewModel.AdvancedSettings;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for GooglePlus.xaml
    /// </summary>
    public partial class GooglePlus : UserControl,INotifyPropertyChanged
    {
        private GooglePlus()
        {
            InitializeComponent();
            MainGrid.DataContext = GooglePlusViewModel;
        }
        static GooglePlus ObJGooglePlus = null;
        public static GooglePlus GetSingeltonGooglePlusObject()
        {
            if (ObJGooglePlus == null)
                ObJGooglePlus = new GooglePlus();
            return ObJGooglePlus;
        }
        private GooglePlusViewModel _googlePlusViewModel = new GooglePlusViewModel();

        public GooglePlusViewModel GooglePlusViewModel
        {
            get
            {
                return _googlePlusViewModel;
            }
            set
            {
                _googlePlusViewModel = value;
                OnPropertyChanged(nameof(GooglePlusViewModel));
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
