using CommonServiceLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.ServiceLocation;

namespace Dominator.Tests.Utils
{
    [TestClass]
    public class UnityInitializationTests
    {
        protected IUnityContainer Container;

        [TestInitialize]
        public virtual void SetUp()
        {
            Container = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));

        }

    }
}
