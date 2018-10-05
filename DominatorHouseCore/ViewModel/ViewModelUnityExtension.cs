using DominatorHouseCore.ViewModel.DashboardVms;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore.ViewModel
{
    public class ViewModelUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IDashboardViewModel, RevisionHistoryViewModel>("RevisionHistory");
        }
    }
}
