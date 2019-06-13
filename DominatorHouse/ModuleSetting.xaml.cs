using DominatorUIUtility.Module;
using Prism.Regions;
using System.Windows;

namespace DominatorHouse
{
    /// <summary>
    /// Interaction logic for ModuleSetting.xaml
    /// </summary>
    public partial class ModuleSetting : Window, IDialogModule
    {

        internal IRegionManager regionManager;
        //public ModuleSetting()
        //{
        //    InitializeComponent();
        //}
        public ModuleSetting(IRegionManager regionManager)
        {
            InitializeComponent();
            this.regionManager = regionManager;
            RegionManager.SetRegionName(this, "StartupRegion");
            RegionManager.SetRegionManager(this, regionManager);
        }
        public bool? ShowModuleSetting()
        {

            try
            {
                ShowDialog();
            }
            catch (System.Exception ex)
            {

                
            }
            return true;
        }
    }
}
