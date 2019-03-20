using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class BooleanToWidthConverterTest
    {
        BooleanToWidthConverter _sut;
        object value;
        object parameter;
        [TestInitialize]
        public void SetUp()
        {
            _sut = new BooleanToWidthConverter();
        }
        [TestMethod]
        public void should_return_zero_if_input_is_true()
        {
            value = true;
            parameter = 1000;
            var result = _sut.Convert(value, value.GetType(), parameter, CultureInfo.CurrentUICulture);
            result.Should().Be(0);
        }
        [TestMethod]
        public void should_return_parameter_value_if_input_is_false_and_parameter_is_not_null()
        {
            value = false;
            parameter = 1000;
            var result = _sut.Convert(value, value.GetType(), parameter, CultureInfo.CurrentUICulture);
            result.Should().Be(parameter);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_return_parameter_value_if_input_is_false_and_parameter_is_null()
        {
            value = false;
            parameter = null;
            var result = _sut.Convert(value, value.GetType(), parameter, CultureInfo.CurrentUICulture);
        }
        [TestMethod]
        public void should_return_Zero_if_input_is_true_and_parameter_is_not_null()
        {
            value = true;
            parameter = 434343;
            var result = _sut.Convert(value, value.GetType(), parameter, CultureInfo.CurrentUICulture);
            result.Should().Be(0);
        }
    }
}
