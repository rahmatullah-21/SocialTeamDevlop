using DominatorUIUtility.ViewModel;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountGrowthControl.xaml
    /// </summary>
    public partial class AccountGrowthControl : UserControl
    {

        public AccountGrowthControl(IAccountGrowthControlViewModel accountGrowthControlViewModel)
        {
            InitializeComponent();
            DataContext = accountGrowthControlViewModel;
        }
    }
}
