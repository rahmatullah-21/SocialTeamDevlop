using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.ViewModel.AdvancedSettings;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Pinterest.xaml
    /// </summary>
    public partial class Pinterest : UserControl
    {
        private Pinterest()
        {
            InitializeComponent();
            MainGrid.DataContext = PinterestViewModel;
        }
        static Pinterest ObjPinterest = null;
        public static Pinterest GetSingeltonPinterestObject()
        {
            if (ObjPinterest == null)
                ObjPinterest = new Pinterest();
            return ObjPinterest;
        }
        private PinterestViewModel _pinterestViewModel = new PinterestViewModel();

        public PinterestViewModel PinterestViewModel
        {
            get
            {
                return _pinterestViewModel;
            }
            set
            {
                _pinterestViewModel = value;
                OnPropertyChanged(nameof(PinterestViewModel));
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
