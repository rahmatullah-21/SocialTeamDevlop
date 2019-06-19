using System.Windows.Controls;
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