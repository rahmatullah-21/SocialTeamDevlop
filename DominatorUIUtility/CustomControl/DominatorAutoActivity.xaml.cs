using LegionUIUtility.ViewModel;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for DominatorAutoActivity.xaml
    /// </summary>
    public partial class DominatorAutoActivity
    {
        public DominatorAutoActivity(IDominatorAutoActivityViewModel activityViewModel)
        {
            InitializeComponent();
            DataContext = activityViewModel;
        }

    }
}
