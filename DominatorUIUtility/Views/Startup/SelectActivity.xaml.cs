using System.Windows.Controls;
using CommonServiceLocator;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
{
    /// <summary>
    /// Interaction logic for SelectActivity.xaml
    /// </summary>
    public partial class SelectActivity : UserControl
    {
        public SelectActivity()
        {
            InitializeComponent();
            var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            SelectActivityType.DataContext = viewModel;
        }
    }
}
