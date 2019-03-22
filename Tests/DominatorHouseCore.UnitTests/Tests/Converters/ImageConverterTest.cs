using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ImageConverterTest
    {
        ImageConverter _sut;
        object value;


        [TestInitialize]
        public void SetUp()
        {
            _sut = new ImageConverter();
        }
        [TestMethod]
        public void should_return_BitmapImage_if_value_is_path_of_file_which_is_present_in_system()
        {
            value = @"C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.jpg";
            var result = (BitmapImage)_sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            var expected = new Uri(value.ToString());
            result.UriSource.AbsolutePath.Should().Be(expected.AbsolutePath);
        }
        [TestMethod]
        public void should_return_NotFound_Image_if_path_is_not_present_in_system()
        {
            value = string.Empty;
            var result = (BitmapImage)_sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            var expected = @"C:/Users/GLB-259/AppData/Local/Socinator/Other/NotFoundImage.png";
            result.UriSource.AbsolutePath.Should().Be(expected);
        }
    }
}
