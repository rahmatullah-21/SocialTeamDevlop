using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class StringPercentageTextConverterTest
    {
        StringPercentageTextConverter _sut;
        object value;

        [TestInitialize]
        public void Setup()
        {
            _sut = new StringPercentageTextConverter();
        }
        [TestMethod]
        public void should_return_value_with_percent_symbole_if_input_is_not_null()
        {
            value = "8";
            var expected = "8 %";
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_null_if_input_is_null()
        {
            value = null;
            object expected = null;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
    }
}
