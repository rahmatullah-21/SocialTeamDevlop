using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for CustomizeAutoActivity.xaml
    /// </summary>
    public partial class CustomizeAutoActivity : UserControl
    {
        public CustomizeAutoActivity(NetworksActivityCustomizeModel model)
        {
            ViewModel = new NetworksActivityCustomizeViewModel(model);
            DataContext = ViewModel;
            InitializeComponent();
        }

        public NetworksActivityCustomizeViewModel ViewModel;

    }
}
