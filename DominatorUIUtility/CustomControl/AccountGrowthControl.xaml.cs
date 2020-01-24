using LegionUIUtility.ViewModel;
using System.Windows.Controls;

namespace LegionUIUtility.CustomControl
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
