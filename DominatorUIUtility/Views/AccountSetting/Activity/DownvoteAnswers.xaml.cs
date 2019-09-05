using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for DownvoteAnswers.xaml
    /// </summary>
    public partial class DownvoteAnswers : UserControl
    {
        public DownvoteAnswers(IDownvoteAnswersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
