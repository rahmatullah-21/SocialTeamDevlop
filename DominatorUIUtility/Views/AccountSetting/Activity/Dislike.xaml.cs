using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Dislike.xaml
    /// </summary>
    public partial class Dislike : UserControl
    {
        public Dislike(IDislikeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
