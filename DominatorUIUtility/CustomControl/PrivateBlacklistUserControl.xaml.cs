using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LegionUIUtility.ViewModel;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for BlacklistUserControl.xaml
    /// </summary>
    public partial class PrivateBlacklistUserControl : UserControl
    {
        public PrivateBlacklistUserControl(IPrivateBlickListViewModel privateBlickListViewModel)
        {
            InitializeComponent();
            MainGrid.DataContext = privateBlickListViewModel;
        }
    }
}
