using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ListCountBoolConverterTest
    {
        ListCountBoolConverter _sut;
        object input;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new ListCountBoolConverter();
        }
        [TestMethod]
        public void should_return_true_if_input_contains_enumerable_data()
        {
            input = new List<string>
            {
               "Kumar","Harsh"
            };
            var expected = true;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_return_false_if_input_contains_non_enumerable_data()
        {
            input = 5;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
        }
        [TestMethod]
        public void should_return_true_if_input_is_null()
        {
            input = null;
            var expected = false;
            var result = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
    }
}
