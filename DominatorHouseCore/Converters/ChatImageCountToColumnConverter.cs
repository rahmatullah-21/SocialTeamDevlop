using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(List<string>), typeof(int))]

    public class ChatImageCountToColumnConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageList = (List<string>)value;

            if (imageList.Count == 1)
                return 1;
            if (imageList.Count == 2 || imageList.Count == 4)
                return 2;
            else
                return 3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
