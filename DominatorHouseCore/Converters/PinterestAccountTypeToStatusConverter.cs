using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class PinterestAccountTypeToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((PinterestAccountType)value).GetDescriptionAttr().FromResourceDictionary();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "LangKeyInActive".FromResourceDictionary();
        }
    }
}
