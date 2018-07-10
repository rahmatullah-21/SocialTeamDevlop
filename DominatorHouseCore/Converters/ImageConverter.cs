using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DominatorHouseCore.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return string.IsNullOrEmpty(value?.ToString()) ? new BitmapImage() : new BitmapImage(new Uri(value.ToString()));
            }
            catch (Exception)
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}