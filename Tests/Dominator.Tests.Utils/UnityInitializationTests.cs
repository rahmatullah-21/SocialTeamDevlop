using CommonServiceLocator;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
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
            Container.RegisterInstance<IDateProvider>(Substitute.For<IDateProvider>());
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));
        }

    }
}
