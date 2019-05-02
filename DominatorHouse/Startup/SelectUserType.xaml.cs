using System.Windows.Controls;
using CommonServiceLocator;
using DominatorUIUtility.ViewModel.Startup;
using DominatorHouse.ViewModels.Startup;

namespace DominatorHouse.Startup
{

    public partial class SelectUserType : UserControl
    {
        public SelectUserType(ISelectUserTypeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

    }
}