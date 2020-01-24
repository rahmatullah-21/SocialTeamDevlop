using System.Windows.Controls;
using LegionUIUtility.Behaviours;

namespace LegionUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for CreateDestination.xaml
    /// </summary>
    public partial class CreateDestination : UserControl
    {
        public CreateDestination()
        {
            InitializeComponent();
        }

        private void OpenContextMenu_OnClick(object sender, System.Windows.RoutedEventArgs e)
            => ViewUtilites.OpenContextMenu(sender);

        private void BtnBackToCampaign_Click(object sender, System.Windows.RoutedEventArgs e)
            => ManageDestinationIndex.Instance.SelectedControl = new ManageDestination();
    }

   
}
