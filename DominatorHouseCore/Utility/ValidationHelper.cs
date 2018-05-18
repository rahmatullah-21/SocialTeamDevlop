using System;
using System.Globalization;
using System.Windows.Controls;

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
    public class IPAddressValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {

                var proxy = value.ToString().Split(':');
                if (!Models.Proxy.IsValidProxy(proxy[0].Trim(), proxy[1].Trim()))
                {
                    return new ValidationResult(false, "Invalid proxy address");
                }
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, "Invalid proxy address");
            }
            
            return new ValidationResult(true, null);

        }

    }
}
