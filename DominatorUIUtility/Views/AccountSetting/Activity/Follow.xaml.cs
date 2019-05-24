using DominatorHouseCore.Enums;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System;
using System.Linq;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Follow.xaml
    /// </summary>
    public partial class Follow : UserControl
    {
        public Follow(IFollowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
