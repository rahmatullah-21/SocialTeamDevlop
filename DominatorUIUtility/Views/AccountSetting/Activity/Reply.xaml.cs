using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Reply.xaml
    /// </summary>
    public partial class Reply : UserControl
    {
        public Reply(IReplyViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
