using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(ObservableCollection<string>), typeof(int))]

    public class ChatImageCountToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageList = (ObservableCollection<string>)value;

            if (imageList.Count == 1)
                return 505;
            if (imageList.Count == 2 || imageList.Count == 4)
                return 246;
            else
                return 160;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
