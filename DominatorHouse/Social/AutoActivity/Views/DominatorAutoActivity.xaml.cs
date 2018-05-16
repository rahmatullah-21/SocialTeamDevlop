using System.Threading.Tasks;
using System.Windows.Controls;
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore.Enums;

namespace DominatorHouse.Social.AutoActivity.Views
{
    /// <summary>
    /// Interaction logic for DominatorAutoActivity.xaml
    /// </summary>
    public partial class DominatorAutoActivity : UserControl
    {
        public DominatorAutoActivityViewModel DominatorAutoActivityViewModel { get; set; }

        private DominatorAutoActivity()
        {
            InitializeComponent();
            DominatorAutoActivityViewModel =  DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();
            DominatorActivityPage.DataContext = DominatorAutoActivityViewModel;
        }

        public static DominatorAutoActivity ObjDominatorAutoActivity = null;

        public static DominatorAutoActivity GetSingletonDominatorAutoActivity(SocialNetworks networks)
        {
            if (ObjDominatorAutoActivity == null)            
                ObjDominatorAutoActivity = new DominatorAutoActivity();

            ObjDominatorAutoActivity.DominatorAutoActivityViewModel.CallRespectiveView(networks);

            return ObjDominatorAutoActivity;
        }

        
    }
}
