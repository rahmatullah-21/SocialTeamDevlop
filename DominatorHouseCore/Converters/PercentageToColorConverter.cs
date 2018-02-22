using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(Brush))]
    public class PercentageToColorConverter : IValueConverter
    {

        public double GivenPercentage { get; set; } 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                GivenPercentage = double.Parse(value.ToString());
            return GivenPercentage >= 25 ?  (GivenPercentage >= 25 && GivenPercentage < 50 ?Brushes.OrangeRed  : (GivenPercentage >= 50 && GivenPercentage < 75  ? Brushes.CornflowerBlue  : Brushes.YellowGreen)): Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}