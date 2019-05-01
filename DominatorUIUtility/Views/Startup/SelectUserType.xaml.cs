using System.Windows.Controls;
using CommonServiceLocator;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
{

    public partial class SelectUserType : UserControl
    {
        ISelectUserTypeViewModel viewModel;
        public SelectUserType()
        {
            InitializeComponent();
             viewModel = ServiceLocator.Current.GetInstance<ISelectUserTypeViewModel>();
            StartupUi.DataContext = viewModel;
        }

    }
}