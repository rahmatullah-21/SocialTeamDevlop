using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        static EnumDescriptionConverter _instance;
        static EnumDescriptionConverter Instance => _instance ?? (_instance = new EnumDescriptionConverter());

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            
            try
            {
                var v = (UserQueryParameters)Enum.Parse(typeof(UserQueryParameters), value.ToString());
                var desc = GetDescription(v);
                return desc.FromResourceDictionary();
            }
            catch(Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return Enum.ToObject(targetType, value);
            }
            catch
            {
                return value;
            }
        }

        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }
    }
}
