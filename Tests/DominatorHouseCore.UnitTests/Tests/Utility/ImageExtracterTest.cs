using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class ImageExtracterTest
    {
        [TestMethod]
        public void should_return_lists_of_imageurl_if_url_is_valid()
        {
            string title = "";
            var result = ImageExtracter.ExtractImageUrls("https://www.google.com/search?q=flower&source=lnms&tbm=isch&sa=X&ved=0ahUKEwjazcy_iZ_hAhXCinAKHbSxBzcQ_AUIDigB&biw=1366&bih=576", ref title);
            result.Should().NotBeEmpty().And.HaveCount(100);
        }
        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void should_throw_UriFormatException_if_url_is_not_valid()
        {
            string title = "";
            ImageExtracter.ExtractImageUrls("url", ref title);
        }
        [TestMethod]

        public void should_return_empty_list_if_url_not_contain_image()
        {
            string title = "";
            var result = ImageExtracter.ExtractImageUrls("http://www.yahoo.com", ref title);
            result.Should().BeEmpty();
        }
        [TestMethod]
        public void should_return_true_if_url_is_image_url()
        {
            var result = ImageExtracter.IsImageUrl("https://i.ytimg.com/vi/ZYifkcmIb-4/maxresdefault.jpg");
            result.Should().BeTrue();
        }
        [TestMethod]
        public void should_return_false_if_url_is_not_image_url()
        {
            var result = ImageExtracter.IsImageUrl("https://youtube.com");
            result.Should().BeFalse();
        }
        [TestMethod]
        public void should_remove_invalid_url()
        {
            string title = "";
            var imageurls = (List<string>)ImageExtracter.ExtractImageUrls("https://www.google.com/search?q=flower&source=lnms&tbm=isch&sa=X&ved=0ahUKEwjazcy_iZ_hAhXCinAKHbSxBzcQ_AUIDigB&biw=1366&bih=576", ref title);
            imageurls.Add("abc");
            imageurls.Add("test");
            imageurls.Add("invalid");
            imageurls.Add("url");
            var result = ImageExtracter.RemoveInvalidUrls(imageurls);
            result.Should().NotBeEmpty().And.HaveCount(100);
        }
    }
}
