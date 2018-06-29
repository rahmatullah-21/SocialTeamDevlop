using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace DominatorHouseCore.Converters
{
    public class AverageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
          
            try
            {
                var sum = 0;
                foreach (var value in values)
                {
                    sum += int.Parse(value.ToString());
                }
                return sum / values.Length;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return 0;
            }

           
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}