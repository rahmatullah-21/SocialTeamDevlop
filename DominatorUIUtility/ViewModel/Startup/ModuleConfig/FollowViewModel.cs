using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IFollowViewModel
    {

    }
    public class FollowViewModel : StartupBaseViewModel, IFollowViewModel
    {
        public FollowViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
        }
    }
}
