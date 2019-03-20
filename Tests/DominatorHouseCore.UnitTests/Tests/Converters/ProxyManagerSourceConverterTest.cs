using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;


namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ProxyManagerSourceConverterTest
    {
        ProxyManagerSourceConverter _sut;
        object[] value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new ProxyManagerSourceConverter();
        }
        [TestMethod]
        public void should_return_empty_colection_if_value_is_need_to_filter_by_group()
        {
             var proxyList = new List<ProxyManagerModel> {
                                              new ProxyManagerModel
                                              {
                                                  Status="Fail",
                                                  AccountsAssignedto=new ObservableCollection<AccountAssign>(),
                                                  AccountProxy=new Proxy
                                                  {
                                                      ProxyGroup="group",
                                                      ProxyIp ="1.0.0.2",
                                                      ProxyName="sample"
                                                  }
                                              }
                                             };
            value = new object[] {
                                    proxyList,
                                    true,
                                    true,
                                    true,
                                    "group",
                                    "filter"
                                };
            
            IEnumerable<ProxyManagerModel> result =(IEnumerable<ProxyManagerModel>)_sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().BeEmpty();
        }
    }
}
