using Microsoft.Practices.Unity.Configuration;
using Unity;
using Unity.Interception.ContainerIntegration;

namespace DominatorHouseCore
{

    public static class IoC
    {
        public static readonly IUnityContainer Container;

        static IoC()
        {
            Container = new UnityContainer();
            Container.AddNewExtension<Interception>();
            Container.AddNewExtension<CoreUnityExtension>();
            Container.LoadConfiguration();
        }
    }
}
