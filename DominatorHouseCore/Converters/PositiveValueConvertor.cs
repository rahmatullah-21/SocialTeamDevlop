using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{

    public class PositiveValueConvertor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (int)value >= 0)
            {
                return value;
            }
            else
            {

                return value != null ? -(int)(value) : 0;
            }
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
