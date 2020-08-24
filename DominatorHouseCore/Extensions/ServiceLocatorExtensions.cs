#region

using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Unity;
using Unity.Resolution;

#endregion

namespace DominatorHouseCore.Extensions
{
    public static class ServiceLocatorExtensions
    {
        public static T ResolveWithDominatorAccount<T>(this IServiceLocator locator, DominatorAccountModel model)
        {
            return locator.GetInstance<IUnityContainer>()
                .Resolve<T>(new ParameterOverride("dominatorAccountModel", model));
        }

        public static IDbOperations ResolveAccountDbOperations(this IServiceLocator locator, string accountId,
            SocialNetworks networks)
        {
            return locator.GetInstance<IUnityContainer>().Resolve<IDbOperations>(
                new ParameterOverride("id", accountId),
                new ParameterOverride("networks", networks),
                new ParameterOverride("type", ConstantVariable.GetAccountDb));
        }

        public static IDbOperations ResolveCampaignDbOperations(this IServiceLocator locator, string accountId,
            SocialNetworks networks)
        {
            return locator.GetInstance<IUnityContainer>().Resolve<IDbOperations>(
                new ParameterOverride("id", accountId),
                new ParameterOverride("networks", networks),
                new ParameterOverride("type", ConstantVariable.GetCampaignDb));
        }
    }
}