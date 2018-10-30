using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace DominatorHouseCore.IntegrationTests
{
    [TestClass]
    public class UnityContainerTests
    {
        private IUnityContainer _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new UnityContainer();
            _sut.AddNewExtension<CoreUnityExtension>();
        }

        [TestMethod]
        public void should_resolve_all_registered_components()
        {
            foreach (var containerRegistration in _sut.Registrations)
            {
                if (string.IsNullOrWhiteSpace(containerRegistration.Name))
                {
                    var aService = _sut.Resolve(containerRegistration.RegisteredType);
                    aService.Should().NotBeNull();
                }
                else
                {
                    var aService = _sut.Resolve(containerRegistration.RegisteredType, containerRegistration.Name);
                    aService.Should().NotBeNull();
                }
            }
        }
    }
}
