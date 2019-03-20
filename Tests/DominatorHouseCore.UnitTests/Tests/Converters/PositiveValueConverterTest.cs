using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows.Media;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class PositiveValueConverterTest
    {
        private PositiveValueConvertor _sut;
        private object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new PositiveValueConvertor();

        }

        [TestMethod]
        public void should_return_positive_value_if_input_is_positive_integer()
        {
            value = 45;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(value);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_return_value_if_input_is_positive_non_integer()
        {
            value = 45.445;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
        }
        [TestMethod]
        public void should_return_positive_value_if_input_is_negetive_integer()
        {
            value = -45;
            var expected = 45;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_zero_if_input_is_null()
        {
            value = null;
            var expected = 0;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
    }
}
