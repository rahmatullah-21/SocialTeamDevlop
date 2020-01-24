using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for PlaceScraper.xaml
    /// </summary>
    public partial class PlaceScraper : UserControl
    {
        public PlaceScraper(IPlaceScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
