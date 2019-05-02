using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouse.ViewModels.Startup;
using MahApps.Metro.Controls;
using Socinator;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace DominatorHouse.Startup
{

    public partial class StartUpMain : MetroWindow
    {
        public StartUpMain()
        {
            DialogParticipation.SetRegister(this, this);
            InitializeComponent();
            var viewModel = ServiceLocator.Current.GetInstance<ISaveSettingViewModel>();
            viewModel.SelectedIndex = 0;
            StratupGrid.DataContext = viewModel;
            Closing += (s, e) =>
            {
                var start = ServiceLocator.Current.GetInstance<MainWindow>();
                start.Show();
            };
        }

    }
}