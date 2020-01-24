using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for MarketPlaceScraper.xaml
    /// </summary>
    public partial class MarketPlaceScraper : UserControl
    {
        public MarketPlaceScraper(IMarketPlaceScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
