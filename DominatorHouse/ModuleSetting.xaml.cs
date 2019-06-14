using DominatorUIUtility.Module;
using Prism.Regions;
using System.Windows;

namespace DominatorHouse
{
    /// <summary>
    /// Interaction logic for ModuleSetting.xaml
    /// </summary>
    public partial class ModuleSetting : Window
    {

        public ModuleSetting(IRegionManager regionManager)
        {
            InitializeComponent();
            RegionManager.SetRegionName(this, "StartupRegion");
            RegionManager.SetRegionManager(this, regionManager);
        }
    }
}
