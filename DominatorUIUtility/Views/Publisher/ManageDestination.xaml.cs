using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for ManageDestination.xaml
    /// </summary>
    public partial class ManageDestination : UserControl
    {       
        public ManageDestination()
        {
            InitializeComponent();                    
        }

        private void ButtonCreateDestination_OnClick(object sender, RoutedEventArgs e)
        {
            ManageDestinationIndex.Instance.SelectedControl = new CreateDestination();
        }
    }
}
