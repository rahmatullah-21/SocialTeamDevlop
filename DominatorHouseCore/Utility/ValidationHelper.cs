using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Models;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace DominatorHouseCore.Utility
{
    public class ValidationHelper : ValidationRule
    {
        public string Sender { get; set; }
        static string ProxyAddress = string.Empty;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                switch (Sender.Substring(3))
                {
                    case "UserName":
                        if (string.IsNullOrEmpty(value?.ToString()))
                        {
                            return new ValidationResult(false, "*Required Field");
                        }
                        break;
                    case "Password":
                        if (string.IsNullOrEmpty(value?.ToString()))
                        {
                            return new ValidationResult(false, "*Required Field");
                        }
                        break;
                    case "ProxyAddress":
                        if (!string.IsNullOrEmpty(value?.ToString()))
                        {
                            ProxyAddress = value.ToString();
                            if (!Models.Proxy.IsValidProxyIp(value.ToString()))
                            {
                                return new ValidationResult(false, "Invalid Address");
                            }
                        }
                        break;
                    case "ProxyPort":
                        if (!string.IsNullOrEmpty(ProxyAddress) && !string.IsNullOrEmpty(value?.ToString()))
                        {
                            if (!Models.Proxy.IsValidProxy(ProxyAddress, value.ToString()))
                            {
                                return new ValidationResult(false, "Invalid Port");
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {

            }
            return new ValidationResult(true, null);
        }

    }
    public class ProxyValidationHelper : ValidationRule
    {
        public string Sender { get; set; }
        static string ProxyAddress = string.Empty;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                switch (Sender.Substring(3))
                {
                    case "ProxyAddress":
                        if (string.IsNullOrEmpty(value as string))
                        {
                            return new ValidationResult(false, "*Required Field");
                        }
                        else
                        {
                            ProxyAddress = value.ToString();
                            if (!Models.Proxy.IsValidProxyIp(value.ToString()))
                            {
                                return new ValidationResult(false, "Invalid Address");
                            }
                        }
                        break;
                    case "ProxyPort":
                        if (string.IsNullOrEmpty(value as string))
                        {
                            return new ValidationResult(false, "*Required Field");
                        }
                        else if (!string.IsNullOrEmpty(ProxyAddress))
                        {
                            //if (!Models.Proxy.IsValidProxy(ProxyAddress, value.ToString()))
                            //{
                            //    return new ValidationResult(false, "Invalid Port");
                            //}

                            if (!Models.Proxy.IsValidProxyPort(value.ToString()))
                            {
                                return new ValidationResult(false, "Invalid Port");
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {

            }
            return new ValidationResult(true, null);
        }

    }
    public class IPAddressValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var proxy = (value as MultiBindingExpression).BindingGroup.Items[1] as Proxy;
                if (!Models.Proxy.IsValidProxy(proxy.ProxyIp, proxy.ProxyPort))
                    return new ValidationResult(false, "Invalid IP address");


            }
            catch (Exception ex)
            {
                return new ValidationResult(false, "Invalid IP address");
            }

            return new ValidationResult(true, null);

        }

    }
    public class UrlValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            try
            {
                if (!Regex.IsMatch(value.ToString(), "(http(s)?://)?(www\\.)+([\\w-]+\\.)+[\\w-]+[.\\w]+(/[/?%&=]*)?"))
                    return new ValidationResult(false, "Invalid URL");
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, "Invalid URL");
            }

            return new ValidationResult(true, string.Empty);
        }
    }
}
