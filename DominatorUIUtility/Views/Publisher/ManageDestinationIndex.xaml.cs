using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for ManageDestinationIndex.xaml
    /// </summary>
    public partial class ManageDestinationIndex : UserControl,INotifyPropertyChanged
    {
        private UserControl _selectedControl = new ManageDestination();

        public UserControl SelectedControl
        {
            get
            {
                return _selectedControl;
            }
            set
            {
                _selectedControl = value;
               OnPropertyChanged(nameof(SelectedControl));
            }
        }

        private static ManageDestinationIndex _instance;

        public static ManageDestinationIndex Instance { get; set; }
            = _instance ?? (_instance=new ManageDestinationIndex());

        private ManageDestinationIndex()
        {
            InitializeComponent();
            DestinationIndex.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
