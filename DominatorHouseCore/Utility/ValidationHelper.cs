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

    public class EndTimeValidator : ValidationRule
    {

        public TimeSpan Min { get; set; }
      
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {

                TimeSpan time = (TimeSpan)value;

                if (TimeSpan.Compare(time, Min) == -1 )
                {
                    return new ValidationResult(false, "Please enter the time more then " + Min + ".");
                }
                else
                {
                    return new ValidationResult(true, null);
                }
            }
            else
                return new ValidationResult(true, null);
        }


    }
    public class StartTimeValidator: ValidationRule
    {
    public TimeSpan Max { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value != null)
        {

            TimeSpan time = (TimeSpan)value;

            if (TimeSpan.Compare(time, Max) == 1)
            {
                return new ValidationResult(false, "Please enter the time less then " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
        else
            return new ValidationResult(true, null);
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
