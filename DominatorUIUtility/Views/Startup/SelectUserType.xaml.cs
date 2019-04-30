
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
{

    public partial class SelectUserType : UserControl
    {
        public SelectUserType()
        {
            InitializeComponent();
            var viewModel = ServiceLocator.Current.GetInstance<ISelectUserTypeViewModel>();
            StartupUi.DataContext = viewModel;
        }

    }
}