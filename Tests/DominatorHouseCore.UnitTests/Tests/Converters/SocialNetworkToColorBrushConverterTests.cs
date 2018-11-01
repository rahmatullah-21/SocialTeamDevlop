using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class SocialNetworkToColorBrushConverterTests
    {
        private SocialNetworkToColorBrushConverter _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new SocialNetworkToColorBrushConverter();
        }

        /// <summary>
        /// this test will failed if a new network will be added but converter isn't changed for this network
        /// </summary>
        [TestMethod]
        public void should_return_value_for_all_the_types()
        {
            // arrange
            var allEnumValues = Enum.GetValues(typeof(SocialNetworks));

            // act
            foreach (var socialNetwork in allEnumValues)
            {
                var val = _sut.Convert(socialNetwork, typeof(SocialNetworks), new object(),
                    CultureInfo.CurrentUICulture);
                // assert
                if (((SocialNetworks)socialNetwork) == SocialNetworks.Social)
                {
                    val.Should().BeNull();
                }
                else
                {
                    val.Should().NotBeNull();
                }
            }
        }
    }
}
