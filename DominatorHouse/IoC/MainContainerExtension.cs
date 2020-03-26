using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouse.ViewModels;
using DominatorUIUtility.ViewModel.Startup;
using DominatorHouseCore.ViewModel;
using Unity;
using Unity.Extension;
using DominatorHouseCore.Interfaces;
using DominatorHouse.Social;

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
            Container.RegisterSingleton<ISocialBrowserManager, SocialBrowserManager>();
            #region Startup ViewModel

            //Container.RegisterType<IFollowViewModel, FollowViewModel>();
            //Container.RegisterType<IUnFollowerViewModel, UnFollowerViewModel>();
            //Container.RegisterType<ILikeViewModel, LikeViewModel>();
            //Container.RegisterType<IUnlikeViewModel, UnlikeViewModel>();
            //Container.RegisterType<ICommentViewModel, CommentViewModel>();
            //Container.RegisterSingleton<ISelectActivityViewModel, SelectActivityViewModel>();
            //Container.RegisterSingleton<ISaveSetting, SaveSetting>();
            //Container.RegisterType<ISendMessageToFollowerViewModel, SendMessageToFollowerViewModel>();
            //Container.RegisterType<IAutoReplyToNewMessageViewModel, AutoReplyToNewMessageViewModel>();  
            //Container.RegisterType<IReplyToCommentViewModel, ReplyToCommentViewModel>();
            //Container.RegisterType<IBlockUserViewModel, BlockUserViewModel>();
            //Container.RegisterType<IDeletePostViewModel, DeletePostViewModel>();
            //Container.RegisterType<IUserScraperViewModel, UserScraperViewModel>();
            //Container.RegisterType<IDownloadScraperViewModel, DownloadScraperViewModel>();
            //Container.RegisterType<IReposterViewModel, ReposterViewModel>();
            //Container.RegisterType<ICommentScraperViewModel, CommentScraperViewModel>();
            //Container.RegisterType<IPostScraperViewModel, PostScraperViewModel>();     
            //Container.RegisterType<IBlockFollowerViewModel, BlockFollowerViewModel>();
            //Container.RegisterType<ILikeCommentViewModel, LikeCommentViewModel>();
            //Container.RegisterType<IHashtagsScraperViewModel, HashtagsScraperViewModel>();
            //Container.RegisterType<IFollowBackViewModel, FollowBackViewModel>();
            #endregion
        }
    }
}
