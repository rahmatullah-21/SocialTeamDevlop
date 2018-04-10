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
using DominatorHouseCore.Utility;

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

        private static ManageDestinationIndex _instance = null;

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
