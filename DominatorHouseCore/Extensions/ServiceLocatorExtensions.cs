using CommonServiceLocator;
using DominatorHouseCore.Models;
using Unity;
using Unity.Resolution;

namespace DominatorHouseCore.Extensions
{
    public static class ServiceLocatorExtensions
    {
        public static T ResolveWithDominatorAccount<T>(this IServiceLocator locator, DominatorAccountModel model)
        {
            return locator.GetInstance<IUnityContainer>().Resolve<T>(new ParameterOverride("dominatorAccountModel", model));
        }
    }
}
