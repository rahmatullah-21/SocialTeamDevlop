using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using DominatorHouseCore.Models;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace DominatorHouseCore.Utility
{
    public class ValidationHelper : ValidationRule
    {
        public string Sender { get; set; }
        private static string ProxyAddress = string.Empty;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var data = value?.ToString().Trim();
                switch (Sender.Substring(3))
                {
                    case "UserName":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        break;
                    case "Password":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        break;
                    case "ProxyAddress":
                        if (!string.IsNullOrEmpty(data))
                        {
                            ProxyAddress = data;
                            if (!Proxy.IsValidProxyIp(data))
                            {
                                return new ValidationResult(false, "LangKeyInvalidAddress".FromResourceDictionary());
                            }
                        }
                        break;
                    case "ProxyPort":
                        //if (!string.IsNullOrEmpty(ProxyAddress) && !string.IsNullOrEmpty(value?.ToString()))
                        //{
                        // ReSharper disable once ConstantConditionalAccessQualifier
                            if (!string.IsNullOrEmpty(data) && !Proxy.IsValidProxyPort(data))
                            {
                                return new ValidationResult(false, "LangKeyInvalidPort".FromResourceDictionary());
                            }
                       // }
                        
                        break;
                    case "EmailUsername":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        break;
                    case "EmailPassword":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        break;
                    case "HostKey":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        break;
                    case "EmailPort":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        break;

                }

                if (Sender == "DatePicker")
                {
                    if (!string.IsNullOrEmpty(data))
                    {
                        var dateTime = (DateTime)value;
                        if (dateTime < DateTime.Today)
                        {                          
                            return new ValidationResult(false, "LangKeyInvalidDate".FromResourceDictionary());
                        }
                    }
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
                var data = value?.ToString().Trim();
                switch (Sender.Substring(3))
                {
                    case "ProxyAddress":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        else
                        {
                            ProxyAddress = data;
                            if (!Proxy.IsValidProxyIp(data))
                            {
                                return new ValidationResult(false, "LangKeyInvalidAddress".FromResourceDictionary());
                            }
                        }
                        break;
                    case "ProxyPort":
                        if (string.IsNullOrEmpty(data))
                        {
                            return new ValidationResult(false, "LangKeyRequiredField".FromResourceDictionary());
                        }
                        else if (!string.IsNullOrEmpty(ProxyAddress))
                        {
                            //if (!Models.Proxy.IsValidProxy(ProxyAddress, value.ToString()))
                            //{
                            //    return new ValidationResult(false, "Invalid Port");
                            //}

                            if (!Proxy.IsValidProxyPort(data))
                            {
                                return new ValidationResult(false, "LangKeyInvalidPort".FromResourceDictionary());
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
                if (!Proxy.IsValidProxy(proxy.ProxyIp, proxy.ProxyPort))
                    return new ValidationResult(false, "LangKeyInvalidIpAddress".FromResourceDictionary());


            }
            catch (Exception)
            {
                return new ValidationResult(false, "LangKeyInvalidIpAddress".FromResourceDictionary());
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
            catch (Exception)
            {
                return new ValidationResult(false, "LangKeyInvalidURL".FromResourceDictionary());
            }

            return new ValidationResult(true, string.Empty);
        }
    }
    public class ValidationBinding : FrameworkElement
    {
        public static readonly DependencyProperty InProperty;
        public static readonly DependencyProperty OutProperty;

        public ValidationBinding()
        {
            Visibility = Visibility.Collapsed;
        }

        static ValidationBinding()
        {
            var inMetadata = new FrameworkPropertyMetadata(
              delegate (DependencyObject p, DependencyPropertyChangedEventArgs args)
              {
                  if (null != BindingOperations.GetBinding(p, OutProperty))
                      (p as ValidationBinding).Out = args.NewValue;
              });

            inMetadata.BindsTwoWayByDefault = false;
            inMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            InProperty = DependencyProperty.Register("In",
                                                     typeof(object),
                                                     typeof(ValidationBinding),
                                                     inMetadata);

            var outMetadata = new FrameworkPropertyMetadata(
              delegate (DependencyObject p, DependencyPropertyChangedEventArgs args)
              {
                  ValueSource source = DependencyPropertyHelper.GetValueSource(p, args.Property);

                  if (source.BaseValueSource != BaseValueSource.Local)
                  {
                      ValidationBinding validationBinding = p as ValidationBinding;
                      object expected = validationBinding.In;
                      if (!ReferenceEquals(args.NewValue, expected))
                      {
                          Dispatcher.CurrentDispatcher.BeginInvoke(
                      DispatcherPriority.DataBind, new Action(delegate
                      {
                          validationBinding.Out = validationBinding.In;
                      }));
                      }
                  }
              });

            outMetadata.BindsTwoWayByDefault = true;
            outMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            OutProperty = DependencyProperty.Register("Out", typeof(object), typeof(ValidationBinding), outMetadata);
        }

        public object In
        {
            get { return GetValue(InProperty); }
            set { SetValue(InProperty, value); }
        }

        public object Out
        {
            get { return GetValue(OutProperty); }
            set { SetValue(OutProperty, value); }
        }
    }
}
