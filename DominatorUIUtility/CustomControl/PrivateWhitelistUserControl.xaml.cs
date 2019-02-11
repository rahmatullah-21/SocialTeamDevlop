using System.Windows.Controls;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for WhitelistuserControl.xaml
    /// </summary>
    public partial class PrivateWhitelistUserControl : UserControl
    {
        public PrivateWhitelistUserControl(IPrivateWhiteListViewModel privateWhiteListViewModel)
        {
            InitializeComponent();
            MainGrid.DataContext = privateWhiteListViewModel;
        }

    }
}
