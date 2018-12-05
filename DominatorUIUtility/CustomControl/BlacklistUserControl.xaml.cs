using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for BlacklistUserControl.xaml
    /// </summary>
    public partial class BlacklistUserControl : UserControl, INotifyPropertyChanged
    {
        public BlacklistUserControl()
        {
            InitializeComponent();
            MainGrid.DataContext = BlackListViewModel;
            BlackListViewModel.InitializeData();
        }

        private BlackListViewModel _blackListViewModel = new BlackListViewModel();

        public BlackListViewModel BlackListViewModel
        {
            get
            {
                return _blackListViewModel;
            }
            set
            {
                if (_blackListViewModel == value)
                    return;
                _blackListViewModel = value;
                OnPropertyChanged(nameof(BlackListViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
