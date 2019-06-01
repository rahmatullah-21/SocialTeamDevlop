using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouse.ViewModels;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel.Startup;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using Unity;
using Unity.Extension;

namespace DominatorHouse.IoC
{
    public class MainContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // views

            // view models
            Container.RegisterSingleton<IMainViewModel, MainViewModel>();
            Container.RegisterSingleton<IPerfCounterViewModel, PerfCounterViewModel>();
            Container.RegisterSingleton<IDominatorAutoActivityViewModel, DominatorAutoActivityViewModel>();

            Container.RegisterSingleton<IApplicationResourceProvider, ApplicationResourceProvider>();

            #region Startup ViewModel

            Container.RegisterType<IFollowViewModel, FollowViewModel>();
            Container.RegisterType<IUnFollowerViewModel, UnFollowerViewModel>();
            Container.RegisterType<ILikeViewModel, LikeViewModel>();
            Container.RegisterType<ICommentViewModel, CommentViewModel>();
            Container.RegisterSingleton<ISelectActivityViewModel, SelectActivityViewModel>();
            Container.RegisterSingleton<ISaveSetting, SaveSetting>();
            

            #endregion

        }
    }
}
