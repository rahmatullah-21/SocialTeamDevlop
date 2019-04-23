using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorHouseCore.Requests;

namespace DominatorHouseCore.UnitTests.Tests.Utility
{
    [TestClass]
    public class WebHelperTest
    {
        [TestMethod]
        public void should_ConvertValidHttpsUrl_method_convert_url_to_valid_http_url()
        {
            
            string expactedresult = "https://www.google.com";
            string data = "www.google.com";
            var result = data.ConvertValidHttpsUrl();

            result.Should().Be(expactedresult);
        }
    }
}
