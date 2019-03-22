using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Windows.Media;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class PercentageToColorConverterTest
    {
        private PercentageToColorConverter _sut;
        private object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new PercentageToColorConverter();
        }

        [TestMethod]
        public void should_return_OrangeRed_brush_if_input_is_in_between_25_and_50()
        {
            value = 45;
            var expacted = Brushes.OrangeRed;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_CornflowerBlue_brush_if_input_is_in_between_50_and_75()
        {
            value = 55;
            var expacted = Brushes.CornflowerBlue;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_YellowGreen_brush_if_input_is_more_than_75()
        {
            value = 200;
            var expacted = Brushes.YellowGreen;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_Red_brush_if_input_is_less_than_25()
        {
            value = 20;
            var expacted = Brushes.Red;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_Red_brush_if_input_is_null()
        {
            value = null;
            var expacted = Brushes.Red;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
    }
}
