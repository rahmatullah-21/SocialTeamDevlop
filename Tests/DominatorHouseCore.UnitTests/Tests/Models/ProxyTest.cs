using DominatorHouseCore.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DominatorHouseCore.UnitTests.Tests.Models
{
    [TestClass]
    public class ProxyTest
    {
        Proxy _proxy;
        [TestInitialize]
        public void SetUp()
        {
            _proxy = new Proxy();
        }
        [TestMethod]
        public void should_return_false_if_proxy_is_not_working()
        {
            _proxy.ProxyIp = "1.9.0.8";
            _proxy.ProxyPort = "12";
            var result = _proxy.CheckProxy();
            result.Should().BeFalse();
        }
        [TestMethod]
        public void should_return_true_if_proxy_is_working()
        {
            _proxy.ProxyIp = "104.144.108.118";
            _proxy.ProxyPort = "3128";
            var result = _proxy.CheckProxy();
            result.Should().BeTrue();
        }
       
        [TestMethod]
        public void should_return_false_if_proxyip_is_not_in_correct_format()
        {
            _proxy.ProxyIp = "9991.9.0.8";
          
            var result = Proxy.IsValidProxyIp(_proxy.ProxyIp);
            result.Should().BeFalse();
        }
        [TestMethod]
        public void should_return_true_if_proxyip_is_in_correct_format()
        {
            _proxy.ProxyIp = "104.144.108.118";
            var result = Proxy.IsValidProxyIp(_proxy.ProxyIp);
            result.Should().BeTrue();
        }
        [TestMethod]
        public void should_return_false_if_proxyport_is_not_in_range()
        {
            _proxy.ProxyPort = "65536";

            var result = Proxy.IsValidProxyPort(_proxy.ProxyPort);
            result.Should().BeFalse();
        }
       
    }
}
