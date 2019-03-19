using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ListViewItemIndexConverterTest
    {
        ListViewItemIndexConverter _sut;
        object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new ListViewItemIndexConverter();
        }
        [TestMethod]
        public void should_return_Zero_if_value_is_any_integer()
        {
        
            value = 1;
            
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);

            result.Should().Be(0);
        }
    }
}
