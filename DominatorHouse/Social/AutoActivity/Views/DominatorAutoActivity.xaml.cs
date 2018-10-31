using DominatorHouse.Social.AutoActivity.ViewModels;

namespace Socinator.Social.AutoActivity.Views
{
    public interface IDominatorAutoActivity
    {
        //IDominatorAutoActivityViewModel DominatorAutoActivityViewModel { get; }
    }
    /// <summary>
    /// Interaction logic for DominatorAutoActivity.xaml
    /// </summary>
    public partial class DominatorAutoActivity : IDominatorAutoActivity
    {
        //public IDominatorAutoActivityViewModel DominatorAutoActivityViewModel { get; set; }

        public DominatorAutoActivity(IDominatorAutoActivityViewModel activityViewModel)
        {
            InitializeComponent();
            DataContext = activityViewModel;
        }

    }
}
