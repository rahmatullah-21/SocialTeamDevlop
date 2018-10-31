using Microsoft.Practices.Unity.Configuration;
using Unity;
using Unity.Interception.ContainerIntegration;

namespace DominatorHouseCore
{

    public static class IoC
    {
        public static IUnityContainer Container { get; private set; }

        public static void Init(IUnityContainer container)
        {

            Container = container;
            Container.AddNewExtension<Interception>();
            Container.AddNewExtension<CoreUnityExtension>();
            Container.LoadConfiguration();
        }
    }
}
