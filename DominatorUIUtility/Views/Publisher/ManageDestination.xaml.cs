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
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for ManageDestination.xaml
    /// </summary>
    public partial class ManageDestination : UserControl, INotifyPropertyChanged
    {
        string DestinationDetailFilePath = ConstantVariable.GetConfigurationDir(DominatorHouseCore.Enums.SocialNetworks.Instagram) + "\\Destinations.bin";

        public ManageDestination()
        {
            InitializeComponent();
            publishersHeader.HeaderText = FindResource("langManageDestination").ToString();
            SetDataContext();
        }

        private void SetDataContext()
        {
            ManageDestinationViewModel = new ManageDestinationViewModel();
            //    ManageDestinationViewModel.ManageDestinationModel.DestinationCollection =
            //     CollectionViewSource.GetDefaultView(ProtoBuffBase.DeserializeObjects<ManageDestinationModel>(DestinationDetailFilePath));
            MainGrid.DataContext = ManageDestinationViewModel.ManageDestinationModel;
        }

        private ManageDestinationViewModel _manageDestinationViewModel;

        public ManageDestinationViewModel ManageDestinationViewModel
        {
            get
            {
                return _manageDestinationViewModel;
            }
            set
            {
                _manageDestinationViewModel = value;
                OnPropertyChanged(nameof(_manageDestinationViewModel));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// OnPropertyChanged is used to notify that some property are changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void btnCreateDestination_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            createDestination.Visibility = System.Windows.Visibility.Visible;
            manageDestination.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnSaveDestination_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //ProtoBuffBase.SerializeObjects(ManageDestinationViewModel.ManageDestinationModel, DestinationDetailFilePath);
            SetDataContext();

        }
    }
}
